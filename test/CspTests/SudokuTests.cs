using Csp.Builders;
using Csp.Gac;
using Csp.Objects.Domain;
using Csp.Objects.Variables.Interfaces;
using Xunit.Abstractions;

namespace CspTests;

public class SudokuTests(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void TestSudoku()
    {
        List<string> grid =
        [
            ".4..2.865",
            "7..6.8...",
            "1....47.2",
            ".1874....",
            "..52.96..",
            "....8615.",
            "9.15....6",
            "...8.2..7",
            "873.6..2.",
        ];

        var sCsp = SudokuBuilder.Create().FromGrid(grid).Build();
        
        // working objects
        var runConstraints = sCsp.Constraints.ToList();
        var runDomains = new Dictionary<IVariable, IDomain<int>>(sCsp.Domains);
        var solvedDomains = Gac.Run(runConstraints, runDomains);

        foreach (var k in solvedDomains.Keys)
        {
            var kDomain = solvedDomains[k].Values;
            Assert.Single(kDomain);
        }
        
        // print final grid
        for (var y = 0; y < 9; y++)
        {
            var line = "";
            for (var x = 0; x < 9; x++)
            {
                var variable = sCsp.Variables.Select(v => (IGridCellVariable)v).First(gcv => gcv.X == x && gcv.Y == y);
                var value = solvedDomains[variable].Values.First();
                line += value;
            }
            testOutputHelper.WriteLine(line);
        }
    }
}