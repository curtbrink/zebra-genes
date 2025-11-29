using Csp.Impl;
using Csp.Impl.Constraints;
using Csp.Impl.Constraints.Selfref;
using Csp.Interfaces;

namespace CspTests.ZebraTest;

public class ZebraTests
{
    [Fact]
    public void TestZebraCsp()
    {
        var riktus = new BaseVariable("Riktus");
        var psyja = new BaseVariable("Psyja");
        List<IVariable> zebraVs = [riktus, psyja];

        var tiger = "Tiger";
        var snep = "Snepfox";
        var domain = new Domain<string>([tiger, snep]);
        
        List<IConstraint<string>> zebraConstraints =
        [
            new NotEqualConstraint(riktus, psyja),
            new EqualsConstraint(riktus, tiger)
        ];

        // immutable csp definition
        var zebraCsp = new UniformDomainCsp<string>(zebraVs, domain, zebraConstraints);

        // make copies for running the thing
        var runConstraints = zebraCsp.Constraints.ToList();
        var runDomains = new Dictionary<IVariable, IDomain<string>>(zebraCsp.Domains);
        Gac.Run(runConstraints, runDomains);
        
        // what's the result?
        Assert.Single(runDomains[riktus].Values);
        Assert.Single(runDomains[psyja].Values);
        Assert.Contains(tiger, runDomains[riktus].Values);
        Assert.Contains(snep, runDomains[psyja].Values);
    }

    [Fact]
    public void TestSelfReferencingVeryEasy()
    {
        var q1 = new BaseVariable("Q1");
        var q2 = new BaseVariable("Q2");
        var q3 = new BaseVariable("Q3");
        var q4 = new BaseVariable("Q4");

        List<IVariable> allQs = [q1, q2, q3, q4];

        var domain = new Domain<string>(["A", "B", "C", "D", "E"]);

        var q1Constraint = new AnswerCountConstraint(q1, allQs, "E", [1, 4, 2, 0, 3]);
        var q2Constraint = new ChoiceEqualsConstraint(q2, q1, ["E", "A", "B", "D", "C"]);
        var q3Constraint = new AnswerCountConstraint(q3, allQs, "B", [2, 0, 4, 3, 1]);
        var q4Constraint = new ChoiceEqualsConstraint(q4, q3, ["D", "B", "A", "E", "C"]);
        
        List<IConstraint<string>> allConstraints = [q1Constraint, q2Constraint, q3Constraint, q4Constraint];

        var csp = new UniformDomainCsp<string>(allQs, domain, allConstraints);
        
        var runConstraints = csp.Constraints.ToList();
        var runDomains = new Dictionary<IVariable, IDomain<string>>(csp.Domains);
        Gac.Run(runConstraints, runDomains);
        
        // what's the result?
        AssertAnswer("A", runDomains[q1]);
        AssertAnswer("B", runDomains[q2]);
        AssertAnswer("E", runDomains[q3]);
        AssertAnswer("D", runDomains[q4]);
    }
    
    [Fact]
    public void TestSelfReferencingEasy()
    {
        var q1 = new OrderedVariable("Q1", 1);
        var q2 = new OrderedVariable("Q2", 2);
        var q3 = new OrderedVariable("Q3", 3);
        var q4 = new OrderedVariable("Q4", 4);
        var q5 = new OrderedVariable("Q5", 5);

        List<IOrderedVariable> allQs = [q1, q2, q3, q4, q5];

        var domain = new Domain<string>(["A", "B", "C", "D", "E"]);

        var q1Constraint = new FirstWithChoiceConstraint(allQs, q1, "E", [2, 5, 3, 4, null]);
        var q2Constraint = new FirstWithChoiceConstraint(allQs, q2, "D", [null, 2, 3, 5, 4]);
        var q3Constraint = new FirstWithChoiceConstraint([q2, q3, q4, q5], q3, "D", [5, 3, 2, 4, null]);
        var q4Constraint = new AnswerCountConstraint(q4, allQs, "A", [3, 0, 5, 2, 4]);
        var q5Constraint = new FirstWithChoiceConstraint([q2, q3, q4, q5], q5, "C", [null, 5, 2, 3, 4]);
        
        List<IConstraint<string>> allConstraints = [q1Constraint, q2Constraint, q3Constraint, q4Constraint, q5Constraint];

        var csp = new UniformDomainCsp<string>(allQs, domain, allConstraints);
        
        var runConstraints = csp.Constraints.ToList();
        var runDomains = new Dictionary<IVariable, IDomain<string>>(csp.Domains);
        Gac.Run(runConstraints, runDomains);
        
        // what's the result?
        AssertAnswer("C", runDomains[q1]);
        AssertAnswer("A", runDomains[q2]);
        AssertAnswer("E", runDomains[q3]);
        AssertAnswer("A", runDomains[q4]);
        AssertAnswer("A", runDomains[q5]);
    }

    private static void AssertAnswer(string expected, IDomain<string> domain)
    {
        Assert.Single(domain.Values);
        Assert.Contains(expected, domain.Values);
    }
}