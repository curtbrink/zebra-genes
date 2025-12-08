using Csp.Builders;
using Csp.Gac;
using Csp.Objects.Domain;
using Csp.Objects.Variables.Interfaces;
using Xunit.Abstractions;

namespace CspTests;

public class AkariTests(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void TestAkariGrid()
    {
        // dailyakari.com/archive/334
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

        PrintGrid(grid, solvedDomains);
    }
    
    [Fact]
    public void TestAkariGridTwo()
    {
        // dailyakari.com/archive/333
        List<string> grid =
        [
            "...............",
            ".ww...1w...1w..",
            "...0....3....2.",
            ".0w...ww...12..",
            "...w....3....w.",
            ".ww...1w...1w..",
            "...............",
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

        PrintGrid(grid, solvedDomains);
    }

    private void PrintGrid(List<string> original, IDictionary<IVariable, IDomain<int>> domains)
    {
        var newGrid = original.Select(v => new string(v).ToCharArray().ToList()).ToList();

        foreach (var (v, domain) in domains)
        {
            var gridCell = (IGridCellVariable)v;
            newGrid[gridCell.Y][gridCell.X] = domain.Values.First() == 1 ? '@' : '.';
        }

        foreach (var row in newGrid)
        {
            testOutputHelper.WriteLine(new string(row.ToArray()));
        }
    }
}