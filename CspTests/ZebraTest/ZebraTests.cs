using Csp.Impl;
using Csp.Impl.Constraints;
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
}