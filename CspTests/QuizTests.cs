using Csp.Builders;
using Csp.Builders.Quiz;
using Csp.Impl;
using Csp.Impl.Constraints.Selfref;
using Csp.Interfaces;

namespace CspTests;

public class QuizTests
{
    [Fact]
    public void TestSelfReferencingVeryEasy()
    {
        var q1 = new BaseVariable("Q1");
        var q2 = new BaseVariable("Q2");
        var q3 = new BaseVariable("Q3");
        var q4 = new BaseVariable("Q4");

        List<IVariable> allQs = [q1, q2, q3, q4];

        var domain = new Domain<string>(["A", "B", "C", "D", "E"]);

        var q1Constraint = new AnswerCountConstraint(q1, allQs, ["E"], [1, 4, 2, 0, 3]);
        var q2Constraint = new ChoiceEqualsConstraint(q2, q1, ["E", "A", "B", "D", "C"]);
        var q3Constraint = new AnswerCountConstraint(q3, allQs, ["B"], [2, 0, 4, 3, 1]);
        var q4Constraint = new ChoiceEqualsConstraint(q4, q3, ["D", "B", "A", "E", "C"]);
        
        List<IConstraint<string>> allConstraints = [q1Constraint, q2Constraint, q3Constraint, q4Constraint];

        var csp = new UniformDomainCsp<string>(allQs, domain, allConstraints);
        
        var runConstraints = csp.Constraints.ToList();
        var runDomains = new Dictionary<IVariable, IDomain<string>>(csp.Domains);
        var solvedDomains = Gac.Run(runConstraints, runDomains);
        
        // what's the result?
        AssertAnswer("A", solvedDomains[q1]);
        AssertAnswer("B", solvedDomains[q2]);
        AssertAnswer("E", solvedDomains[q3]);
        AssertAnswer("D", solvedDomains[q4]);
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

        var q1Constraint = new FirstWithChoiceConstraint(q1, allQs, [2, 5, 3, 4, null], "E");
        var q2Constraint = new FirstWithChoiceConstraint(q2, allQs, [null, 2, 3, 5, 4], "D");
        var q3Constraint = new FirstWithChoiceConstraint(q3, [q2, q3, q4, q5], [5, 3, 2, 4, null], "D");
        var q4Constraint = new AnswerCountConstraint(q4, allQs, ["A"], [3, 0, 5, 2, 4]);
        var q5Constraint = new FirstWithChoiceConstraint(q5, [q2, q3, q4, q5],  [null, 5, 2, 3, 4], "C");
        
        List<IConstraint<string>> allConstraints = [q1Constraint, q2Constraint, q3Constraint, q4Constraint, q5Constraint];

        var csp = new UniformDomainCsp<string>(allQs, domain, allConstraints);
        
        var runConstraints = csp.Constraints.ToList();
        var runDomains = new Dictionary<IVariable, IDomain<string>>(csp.Domains);
        var solvedDomains = Gac.Run(runConstraints, runDomains);
        
        // what's the result?
        AssertAnswer("C", solvedDomains[q1]);
        AssertAnswer("A", solvedDomains[q2]);
        AssertAnswer("E", solvedDomains[q3]);
        AssertAnswer("A", solvedDomains[q4]);
        AssertAnswer("A", solvedDomains[q5]);
    }
    
    [Fact]
    public void TestSelfReferencingMedium()
    {
        var questions = new List<IOrderedVariable>();
        for (var i = 1; i <= 8; i++)
        {
            questions.Add(new OrderedVariable($"Q{i}", i));
        }

        var domain = new Domain<string>(["A", "B", "C", "D", "E"]);

        var constraints = new List<IConstraint<string>>
        {
            // Q1: Which is the first question with answer B?
            new FirstWithChoiceConstraint(questions[0], questions, [5, 6, 8, 2, 1], "B"),
            // Q2: What is the answer to Q5?
            new ChoiceEqualsConstraint(questions[1], questions[4], ["E", "A", "B", "D", "C"]),
            // Q3: Which is the last question with answer C?
            new FirstWithChoiceConstraint(questions[2], questions, [8, 1, 7, null, 2], "C", true),
            // Q4: Which is the last question with answer D?
            new FirstWithChoiceConstraint(questions[3], questions, [6, 8, 1, 2, null], "D", true),
            // Q5: Which is the first question with answer E?
            new FirstWithChoiceConstraint(questions[4], questions,  [8, 4, 5, null, 6], "E"),
            // Q6: How many questions have answer A?
            new AnswerCountConstraint(questions[5], questions, ["A"], [4, 3, 2, 0, 7]),
            // Q7: Which are the only two consecutive questions with the same answer?
            // e.g. A = 5 and 6 have the same answer, and no other adjacent pair of questions in the puzzle has the same answer as each other
            new OnlyConsecutiveSameConstraint(questions[6], questions, [[5, 6], [4, 5], [3, 4], [1, 2], [2, 3]]),
            // Q8: Which is the least common answer?
            new MostLeastCommonConstraint(questions[7], questions, ["E", "A", "D", null, "C"], true)
        };
        
        var csp = new UniformDomainCsp<string>(questions, domain, constraints);
        
        var runConstraints = csp.Constraints.ToList();
        var runDomains = new Dictionary<IVariable, IDomain<string>>(csp.Domains);
        
        // knock out all the direct logical offenders first
        var workingDomains = Gac.Run(runConstraints, runDomains);

        // now use search to find our solution
        var solvedDomains = Gac.RunWithBacktrackingSearch(runConstraints, workingDomains);
        
        // solving without checking first! :o
        foreach (var d in solvedDomains.Values)
        {
            Assert.Single(d.Values);
        }
        
        // ok so this is the point where we'd require some search and backtracking.
        // GAC alone was only able to rule out 2 choices on 3 questions, 1 choice on 3 questions, none on the other 2.
    }

    [Fact]
    public void AnotherSelfRefVeryEasyTest()
    {
        var questions = new List<IOrderedVariable>();
        for (var i = 1; i <= 4; i++)
        {
            questions.Add(new OrderedVariable($"Q{i}", i));
        }

        var domain = new Domain<string>(["A", "B", "C", "D", "E"]);

        var constraints = new List<IConstraint<string>>
        {
            // Q1: What is the answer to Q2?
            new ChoiceEqualsConstraint(questions[0], questions[1], ["B", "C", "D", "A", "E"]),
            // Q2: How many questions are answer C?
            new AnswerCountConstraint(questions[1], questions, ["C"], [0, 2, 4, 1, 3]),
            // Q3: What is the answer to Q1?
            new ChoiceEqualsConstraint(questions[2], questions[0], ["A", "E", "B", "D", "C"]),
            // Q4: How many questions are answer A?
            new AnswerCountConstraint(questions[3], questions, ["A"], [0, 1, 4, 2, 3])
        };
        
        var csp = new UniformDomainCsp<string>(questions, domain, constraints);
        
        var runConstraints = csp.Constraints.ToList();
        var runDomains = new Dictionary<IVariable, IDomain<string>>(csp.Domains);
        
        // knock out all the direct logical offenders first
        var workingDomains = Gac.Run(runConstraints, runDomains);

        // now use search to find our solution
        var solvedDomains = Gac.RunWithBacktrackingSearch(runConstraints, workingDomains);
        
        // solving without checking first! :o
        foreach (var d in solvedDomains.Values)
        {
            Assert.Single(d.Values);
        }
    }
    
    [Fact]
    public void TestSelfReferencingHard()
    {
        var questions = new List<IOrderedVariable>();
        for (var i = 1; i <= 10; i++)
        {
            questions.Add(new OrderedVariable($"Q{i}", i));
        }

        var domain = new Domain<string>(["A", "B", "C", "D", "E"]);

        var constraints = new List<IConstraint<string>>
        {
            // Q1: Which is the last question with answer D?
            new FirstWithChoiceConstraint(questions[0], questions, [null, 6, 9, 4, 3], "D", true),
            // Q2: How many questions are answer D?
            new AnswerCountConstraint(questions[1], questions, ["D"], [6, 0, 1, 3, 10]),
            // Q3: Which is the least common answer?
            new MostLeastCommonConstraint(questions[2], questions, ["E", null, "C", "A", "D"], true),
            // Q4: How many questions are answer E?
            new AnswerCountConstraint(questions[3], questions, ["E"], [4, 0, 8, 6, 1]),
            // Q5: Which is the last question with answer A?
            new FirstWithChoiceConstraint(questions[4], questions, [10, 3, 1, 5, 9], "A", true),
            // Q6: How many questions are answer B?
            new AnswerCountConstraint(questions[5], questions, ["B"], [0, 6, 3, 4, 1]),
            // Q7: What is the answer to Q4?
            new ChoiceEqualsConstraint(questions[6], questions[3], ["E", "D", "B", "C", "A"]),
            // Q8: Which is the last question before Q9 with answer B?
            new FirstWithChoiceConstraint(questions[7], questions[..8], [8, 4, 5, 2, 3], "B", true),
            // Q9: How many questions are answer A?
            new AnswerCountConstraint(questions[8], questions, ["A"], [6, 1, 0, 3, 4]),
            // Q10: Which is the first question with answer E?
            new FirstWithChoiceConstraint(questions[9], questions, [3, 1, null, 2, 4], "E")
        };
        
        var csp = new UniformDomainCsp<string>(questions, domain, constraints);
        
        var runConstraints = csp.Constraints.ToList();
        var runDomains = new Dictionary<IVariable, IDomain<string>>(csp.Domains);
        
        // knock out all the direct logical offenders first
        var workingDomains = Gac.Run(runConstraints, runDomains);

        // now use search to find our solution
        var solvedDomains = Gac.RunWithBacktrackingSearch(runConstraints, workingDomains);
        
        // solving without checking first! :o
        foreach (var d in solvedDomains.Values)
        {
            Assert.Single(d.Values);
        }
    }
    
    [Fact]
    public void TestQuizBuilder1()
    {
        var qb = QuizBuilder.New(5)
            .WhatIsThe().AnswerToQuestion(3).WithChoices("D", "A", "B", "E", "C").EndQuestion()
            .WhatIsThe().CountOfAnswer("E").WithChoices(1, 4, 3, 0, 2).EndQuestion()
            .WhatIsThe().CountOfAnswer("C").WithChoices(2, 3, 0, 1, 4).EndQuestion()
            .WhatIsThe().AnswerToQuestion(2).WithChoices("C", "A", "D", "B", "E").EndQuestion()
            .Build();
        
        var workingDomains = new Dictionary<IVariable, IDomain<string>>(qb.Domains);
        var workingConstraints = qb.Constraints.ToList();
        
        var solvedDomains = Gac.Run(workingConstraints, workingDomains);
    }
    
    [Fact]
    public void TestQuizBuilder2()
    {
        var qb = QuizBuilder.New(5)
            .WhatIsThe().First().WithAnswer("D").WithChoices(2, 1, null, 4, 3).EndQuestion()
            .WhatIsThe().CountOfAnswer("B").WithChoices(3, 4, 1, 0, 2).EndQuestion()
            .WhatIsThe().AnswerToQuestion(1).WithChoices("C", "B", "A", "D", "E").EndQuestion()
            .WhatIsThe().AnswerToQuestion(3).WithChoices("E", "D", "C", "A", "B").EndQuestion()
            .WhatIsThe().First().WithAnswer("A").After(1).WithChoices(3, 4, null, 5, 2).EndQuestion()
            .Build();
        
        var workingDomains = new Dictionary<IVariable, IDomain<string>>(qb.Domains);
        var workingConstraints = qb.Constraints.ToList();
        
        var solvedDomains = Gac.Run(workingConstraints, workingDomains);
    }
    
    [Fact]
    public void TestQuizBuilder3()
    {
        var qb = QuizBuilder.New(5)
            .WhatIsThe().CountOfAnswer("A").WithChoices(3, 1, 5, 4, 2).EndQuestion()
            .WhatIsThe().OnlyConsecutiveSameSetOf(2).WithChoices(5, 6, 2, 7, 4).EndQuestion()
            .WhatIsThe().First().WithAnswer("E").WithChoices(null, 3, 1, 6, 8).EndQuestion()
            .WhatIsThe().First().WithAnswer("A").WithChoices(4, 8, 1, 3, 2).EndQuestion()
            .WhatIsThe().First().WithAnswer("A").After(2).WithChoices(5, 3, 4, 8, null).EndQuestion()
            .WhatIsThe().Last().WithAnswer("B").WithChoices(5, 7, 8, null, 3).EndQuestion()
            .WhatIsThe().AnswerToQuestion(7).WithChoices("D", "E", "C", "B", "A").EndQuestion()
            .WhatIsThe().OnlyQuestionWithTheSameAnswer().WithChoices(2, 1, 5, 7, 4).EndQuestion()
            .Build();
        
        var workingDomains = new Dictionary<IVariable, IDomain<string>>(qb.Domains);
        var workingConstraints = qb.Constraints.ToList();
        
        var domainsToBacktrack = Gac.Run(workingConstraints, workingDomains);

        var solvedDomains = Gac.RunWithBacktrackingSearch(workingConstraints, domainsToBacktrack);
    }
    
    [Fact]
    public void TestQuizBuilder4()
    {
        var qb = QuizBuilder.New(5)
            .WhatIsThe().First().WithAnswer("E").After(2).WithChoices(3, 10, 9, 5, null).EndQuestion()
            .WhatIsThe().First().WithAnswer("B").After(6).WithChoices(7, 9, 10, null, 8).EndQuestion()
            .WhatIsThe().CountOfAnswer("B").WithChoices(3, 2, 6, 5, 1).EndQuestion()
            .WhatIsThe().OnlyConsecutiveSameSetOf(2).WithChoices(4, 9, 7, 1, 5).EndQuestion()
            .WhatIsThe().First().WithAnswer("C").After(2).WithChoices(3, 7, 5, 4, null).EndQuestion()
            .WhatIsThe().Last().WithAnswer("E").Before(9).WithChoices(8, 5, 1, 2, 7).EndQuestion()
            .WhatIsThe().CountOfAnswer("C").WithChoices(10, 1, 2, 7, 3).EndQuestion()
            .WhatIsThe().CountOfAnswer("D").WithChoices(9, 4, 0, 7, 1).EndQuestion()
            .WhatIsThe().First().WithAnswer("D").WithChoices(8, 3, 5, 2, 6).EndQuestion()
            .WhatIsThe().AnswerToQuestion(2).WithChoices("D", "C", "B", "E", "A").EndQuestion()
            .Build();
        
        var workingDomains = new Dictionary<IVariable, IDomain<string>>(qb.Domains);
        var workingConstraints = qb.Constraints.ToList();
        
        var domainsToBacktrack = Gac.Run(workingConstraints, workingDomains);

        var solvedDomains = Gac.RunWithBacktrackingSearch(workingConstraints, domainsToBacktrack);
    }
    
    [Fact]
    public void TestQuizBuilder5()
    {
        var qb = QuizBuilder.New(5)
            .WhatIsThe().Last().Before(7).WithAnswer("E").WithChoices(4, null, 2, 1, 6).EndQuestion()
            // only true statement but need dummy
            .WhatIsThe().AnswerToQuestion(2).WithChoices("A", "B", "C", "D", "E").EndQuestion()
            .WhatIsThe().AnswerToQuestion(2).WithChoices("B", "A", "E", "D", "C").EndQuestion()
            .WhatIsThe().Last().WithSameAnswer().WithChoices(12, 9, 11, 10, 4).EndQuestion()
            .WhatIsThe().AnswerToQuestion(9).WithChoices("E", "B", "D", "A", "C").EndQuestion()
            .WhatIsThe().CountOfVowels().WithChoices(12, 9, 8, 3, 11).EndQuestion()
            .WhatIsThe().CountOfMostCommonAnswer().WithChoices(4, 10, 5, 11, 6).EndQuestion()
            // only true statement but need dummy
            .WhatIsThe().AnswerToQuestion(8).WithChoices("A", "B", "C", "D", "E").EndQuestion()
            .WhatIsThe().MostCommonAnswer().WithChoices("D", "E", "C", null, "A").EndQuestion()
            .WhatIsThe().First().After(1).WithAnswer("E").WithChoices(10, 4, 12, 8, 6).EndQuestion()
            .WhatIsThe().CountOfAnswer("E").Before(12).WithChoices(5, 3, 10, 4, 0).EndQuestion()
            .WhatIsThe().Previous().WithSameAnswer().WithChoices(11, 6, 2, 10, 7).EndQuestion()
            .Build();
        
        var workingDomains = new Dictionary<IVariable, IDomain<string>>(qb.Domains);
        var workingConstraints = qb.Constraints.ToList();
        
        var domainsToBacktrack = Gac.Run(workingConstraints, workingDomains);

        var solvedDomains = Gac.RunWithBacktrackingSearch(workingConstraints, domainsToBacktrack);
    }

    public void Playground()
    {
        // doesn't run just checking syntax
        var q = QuizBuilder.New(5)
            .WhatIsThe().Next().WithSameAnswer().WithChoices(5, 6, 7, 8, 9).EndQuestion();
    }
    
    private static void AssertAnswer(string expected, IDomain<string> domain)
    {
        Assert.Single(domain.Values);
        Assert.Contains(expected, domain.Values);
    }
}