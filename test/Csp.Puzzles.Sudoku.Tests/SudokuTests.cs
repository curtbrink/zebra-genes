using Csp.Puzzles.Sudoku.Builders;
using Xunit.Abstractions;

namespace Csp.Puzzles.Sudoku.Tests;

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

        Assert.Equal(81, sCsp.Variables.Count);
        Assert.Equal(27, sCsp.Constraints.Count);
    }
}