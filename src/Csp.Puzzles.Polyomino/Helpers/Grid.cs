namespace Csp.Puzzles.Polyomino.Helpers;

public class Grid
{
    private readonly bool[,] _grid;

    public Grid(List<string> input, char occupiedChar)
    {
        var h = input.Count;
        if (h == 0) throw new ArgumentOutOfRangeException(nameof(input), "Can't be empty");

        var w = input[0].Length;
        if (w == 0) throw new ArgumentOutOfRangeException(nameof(input), "Can't be empty");

        _grid = new bool[h, w];
        for (var y = 0; y < h; y++)
        {
            for (var x = 0; x < w; x++)
            {
                _grid[y, x] = input[y][x] == occupiedChar;
            }
        }
    }

    public bool[,] GetGrid() => _grid;

    public bool[,] GetGridRotatedLeft() => GetGridRotatedLeftNTimes(_grid, 1);
    
    public bool[,] GetGridRotated180() => GetGridRotatedLeftNTimes(_grid, 2);

    public bool[,] GetGridRotatedRight() => GetGridRotatedLeftNTimes(_grid, 3);

    public bool[,] GetMirroredGridRotatedLeft() => GetGridRotatedLeftNTimes(GetMirroredGrid(), 1);
    
    public bool[,] GetMirroredGridRotated180() => GetGridRotatedLeftNTimes(GetMirroredGrid(), 2);
    
    public bool[,] GetMirroredGridRotatedRight() => GetGridRotatedLeftNTimes(GetMirroredGrid(), 3);

    private static bool[,] GetGridRotatedLeftNTimes(bool[,] grid, int n)
    {
        var resultGrid = grid;
        for (var i = 0; i < n; i++)
        {
            var w = resultGrid.GetLength(1);
            var h = resultGrid.GetLength(0);
            // make grid with sizes rotated
            var newGrid = new bool[w, h];
            
            // iterate columns first
            for (var x = 0; x < w; x++)
            {
                // right to left, though
                var gridIdxX = w - 1 - x;
                for (var y = 0; y < h; y++)
                {
                    newGrid[x, y] = resultGrid[y, gridIdxX];
                }
            }

            resultGrid = newGrid;
        }

        return resultGrid;
    }

    public bool[,] GetMirroredGrid()
    {
        var w = _grid.GetLength(0);
        var h = _grid.GetLength(1);
        // make grid with sizes rotated
        var newGrid = new bool[w, h];

        for (var y = 0; y < h; y++)
        {
            for (var x = 0; x < w; x++)
            {
                var mirrorIdx = w - 1 - x;
                newGrid[y, x] = _grid[y, mirrorIdx];
            }
        }

        return newGrid;
    }
    
    public static string PrettyPrintGrid(bool[,] grid)
    {
        var s = "";
        for (var i = 0; i < grid.GetLength(0); i++)
        {
            var line = "";
            for (var j = 0; j < grid.GetLength(1); j++)
            {
                line += grid[i, j] ? "#" : ".";
            }

            line += '\n';
            s += line;
        }

        return s;
    }
    
}

public static class GridExtensions
{
    public static List<(int x, int y)> GetOrthogonalNeighbors(this bool[,] grid, int x, int y) =>
        new List<(int x, int y)> { (x, y - 1), (x + 1, y), (x, y + 1), (x - 1, y) }.Where(c =>
            c.x >= 0 && c.x < grid.GetLength(1) && c.y >= 0 && c.y < grid.GetLength(0)).ToList();
}