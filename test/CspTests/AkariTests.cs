using Csp.Builders;
using Csp.Gac;
using Csp.Objects.Domain;
using Csp.Objects.Variables.Interfaces;

namespace CspTests;

public class AkariTests
{
    [Fact]
    public void TestAkariGrid()
    {
        List<string> grid =
        [
            "www.ww.w.w",
            "ww1.w1.1.w",
            "w........w",
            "1........1",
            "..1wwww1..",
            "....1w1...",
            "..........",
            "..........",
            "w1..w.w.1w",
            "ww..1.1.ww",
        ];

        var akariCsp = AkariBuilder.Create().FromGrid(grid).Build();
        
        // working objects
        var runConstraints = akariCsp.Constraints.ToList();
        var runDomains = new Dictionary<IVariable, IDomain<int>>(akariCsp.Domains);
        
        var domainsToBacktrack = Gac.Run(runConstraints, runDomains);

        var solvedDomains = Gac.RunWithBacktrackingSearch(runConstraints, domainsToBacktrack);

        foreach (var k in solvedDomains.Keys)
        {
            var kDomain = solvedDomains[k].Values;
            Assert.Single(kDomain);
        }
    }
}