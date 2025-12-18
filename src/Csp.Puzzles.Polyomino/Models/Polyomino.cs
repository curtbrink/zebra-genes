using Csp.Puzzles.Polyomino.Helpers;

namespace Csp.Puzzles.Polyomino.Models;

public class Polyomino
{
    public string Id { get; init; }
    public int TileCount { get; }
    public Grid Definition => _gridDefinition;
    public List<List<(int x, int y)>> PossiblePlacements => _allPlacements;
    
    private readonly Grid _gridDefinition;
    private readonly List<List<(int x, int y)>> _allPlacements;

    public Polyomino(string typeId, List<string> gridDefinition)
    {
        Id = typeId;
        _gridDefinition = new Grid(gridDefinition, '#');
        _allPlacements = BuildAllPlacements(_gridDefinition);
        TileCount = _allPlacements[0].Count;
    }

    private static List<List<(int x, int y)>> BuildAllPlacements(Grid grid)
    {
        List<bool[,]> grids =
        [
            grid.GetGrid(),
            grid.GetGridRotatedLeft(),
            grid.GetGridRotated180(),
            grid.GetGridRotatedRight(),
            grid.GetMirroredGrid(),
            grid.GetMirroredGridRotatedLeft(),
            grid.GetMirroredGridRotated180(),
            grid.GetMirroredGridRotatedRight(),
        ];

        var placements = grids.Select(BuildPlacementFromGrid).ToList();
        var uniques = DeduplicatePlacements(placements);

        return uniques;
    }

    private static List<(int x, int y)> BuildPlacementFromGrid(bool[,] grid)
    {
        var coords = new List<(int x, int y)>();
        for (var y = 0; y < grid.GetLength(0); y++)
        {
            for (var x = 0; x < grid.GetLength(1); x++)
            {
                if (grid[y, x]) coords.Add((x, y));
            }
        }

        return coords;
    }

    private static List<List<(int x, int y)>> DeduplicatePlacements(List<List<(int x, int y)>> placements)
    {
        List<List<(int x, int y)>> uniques = [];

        foreach (var p in placements)
        {
            if (!uniques.Any(u => u.SequenceEqual(p)))
            {
                uniques.Add(p);
            }
        }

        return uniques;
    }
}