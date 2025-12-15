using Csp.Puzzles.Akari.Builders;
using Xunit.Abstractions;

namespace Csp.Puzzles.Akari.Tests;

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
        
        Assert.Equal(110, akariCsp.Constraints.Count);
        Assert.Equal(61, akariCsp.Variables.Count);
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
        
        Assert.Equal(153, akariCsp.Constraints.Count);
        Assert.Equal(81, akariCsp.Variables.Count);
    }
}