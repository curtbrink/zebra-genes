using Csp.Core.Solvers.Shared.Interfaces;
using Csp.Puzzles.Polyomino.Helpers;
using Csp.Puzzles.Polyomino.Models;
using Microsoft.Extensions.Logging;

namespace Csp.Puzzles.Polyomino.Pruners;

public class HoleAnalysisPruner(ILogger<HoleAnalysisPruner> logger) : IPruner<ISearchState<Placement>, Placement>
{
    private PolyominoSearchState? _searchState;

    private Queue<(int x, int y)> _queue = new();

    private int[] _visited = new int[1];
    private int _visitedGen;
    private int _gridTileCount;
    private int _totalTilesToPlace;

    private readonly List<(int x, int y)> _placedCoordinates = [];
    private readonly Dictionary<Models.Polyomino, int> _tileCounts = new();
    private int _smallestRemainingPieceSize;
    private int _largestRemainingPieceSize;

    public void SetSearchState(ISearchState<Placement>? searchState)
    {
        if (searchState is not PolyominoSearchState pState) throw new ArgumentOutOfRangeException(nameof(searchState));
        _searchState = pState;
        
        _queue = new Queue<(int x, int y)>();
        _gridTileCount = pState.GridHeight * pState.GridWidth;
        _totalTilesToPlace = pState.Pieces.Select(p => p.TileCount * pState.Quotas[p]).Sum();
        _visited = new int[_gridTileCount];
        _visitedGen = 0;
    }
    
    public bool ShouldPrune()
    {
        if (_searchState?.DomainStore is null) return false;

        _visitedGen++;
        
        // if we haven't placed enough tiles don't bother with the expensive stuff.
        var shouldShortCircuit = GetFirmPlacements();
        if (shouldShortCircuit) return false;

        if (_largestRemainingPieceSize == 0 && _smallestRemainingPieceSize == 0)
            return false;
        
        var spaceYetRequired = _totalTilesToPlace - _placedCoordinates.Count;

        var usableEmptyTileCount = _gridTileCount - _placedCoordinates.Count;

        if (usableEmptyTileCount < spaceYetRequired) return true; // impossible!
        
        // find all holes
        var largestHoleSize = 0;
        for (var x = 0; x < _searchState.GridWidth; x++)
        {
            for (var y = 0; y < _searchState.GridHeight; y++)
            {
                var idx = y * _searchState.GridWidth + x;
                if (_visited[idx] == _visitedGen) continue; // skip if visited already
                var holeSize = FloodHole(x, y, idx);
                if (holeSize == 0) continue;

                if (holeSize < _smallestRemainingPieceSize)
                {
                    usableEmptyTileCount -= holeSize;
                    if (usableEmptyTileCount < spaceYetRequired) return true;
                }

                if (holeSize > largestHoleSize) largestHoleSize = holeSize;
            }
        }

        if (largestHoleSize < _largestRemainingPieceSize) return true; // impossible
        
        // that gets us the basics as a start.
        return false;
    }

    private int FloodHole(int x, int y, int idx)
    {
        var holeSize = 0;
        _visited[idx] = _visitedGen;
        
        if (_placedCoordinates.Contains((x, y))) return holeSize; // not a hole
        
        _queue.Enqueue((x, y));
        
        while (_queue.Count > 0)
        {
            var newCell = _queue.Dequeue();
            holeSize++;
            
            List<(int x, int y)> neighbors =
            [
                (newCell.x - 1, newCell.y), (newCell.x, newCell.y - 1), (newCell.x + 1, newCell.y),
                (newCell.x, newCell.y + 1)
            ];

            foreach (var n in neighbors)
            {
                if (n.x < 0 || n.x >= _searchState!.GridWidth || n.y < 0 || n.y >= _searchState!.GridHeight) continue;
                var nIdx = n.y * _searchState.GridWidth + n.x;
                if (_visited[nIdx] == _visitedGen) continue;

                _visited[nIdx] = _visitedGen;
                if (_placedCoordinates.Contains(n)) continue;
                
                _queue.Enqueue(n);
            }
        }

        logger.LogDebug("Found a hole of size {0}", holeSize);
        return holeSize;
    }

    private bool GetFirmPlacements()
    {
        _placedCoordinates.Clear();
        _tileCounts.Clear();
        foreach (var kvp in _searchState!.DomainStore!.GetAllDomains())
        {
            if (kvp.Value.Values.Count != 1) continue;
            var p = kvp.Value.Values.First();

            foreach (var c in p.P.PossiblePlacements[p.Variation])
            {
                _tileCounts.TryAdd(p.P, 0);
                _tileCounts[p.P]++;
                _placedCoordinates.Add((c.x + p.X, c.y + p.Y));
            }
        }
        
        // less than 50% placed.
        if (_placedCoordinates.Count < _gridTileCount / 2) return true;

        _smallestRemainingPieceSize = 999;
        _largestRemainingPieceSize = 0;
        var piecesRemaining = false;
        foreach (var piece in _searchState.Pieces)
        {
            if (_searchState.Quotas[piece] * piece.TileCount - _tileCounts.GetValueOrDefault(piece, 0) > 0)
            {
                piecesRemaining = true;
                if (piece.TileCount < _smallestRemainingPieceSize) _smallestRemainingPieceSize = piece.TileCount;
                if (piece.TileCount > _largestRemainingPieceSize) _largestRemainingPieceSize = piece.TileCount;
            }
        }
        if (!piecesRemaining) _smallestRemainingPieceSize = 0;

        return false;
    }
}