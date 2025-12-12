using Csp.Builders;
using Csp.Gac;
using Csp.Objects.Domain;
using Csp.Objects.Variables.Interfaces;
using Xunit.Abstractions;

namespace CspTests;

public class TangoTests(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void TestTango()
    {
        List<string> grid =
        [
            "......",
            "......",
            "......",
            "1.2.12",
            "221121",
            "2112..",
        ];

        var t = TangoBuilder.Create(6).FromGrid(grid)
            .AddEqualsConstraint((4, 1), (5, 1))
            .AddNotEqualConstraint((5, 1), (5, 2))
            .AddNotEqualConstraint((1, 2), (2, 2))
            .AddNotEqualConstraint((1, 2), (1, 3))
            .AddEqualsConstraint((2, 2), (2, 3))
            .AddNotEqualConstraint((4, 2), (5, 2))
            .AddEqualsConstraint((1, 3), (2, 3))
            .Build();
        
        // working objects
        var runConstraints = t.Constraints.ToList();
        var runDomains = new Dictionary<IVariable, IDomain<int>>(t.Domains);
        var domainsToBacktrack = Gac.Run(runConstraints, runDomains);

        var solvedDomains = Gac.RunWithBacktrackingSearch(runConstraints, domainsToBacktrack);

        foreach (var k in solvedDomains.Keys)
        {
            var kDomain = solvedDomains[k].Values;
            Assert.Single(kDomain);
        }
        
        // print final grid
        for (var y = 0; y < 6; y++)
        {
            var line = "";
            for (var x = 0; x < 6; x++)
            {
                var variable = t.Variables.Select(v => (IGridCellVariable)v).First(gcv => gcv.X == x && gcv.Y == y);
                var value = solvedDomains[variable].Values.First();
                line += value;
            }
            testOutputHelper.WriteLine(line);
        }
    }
}