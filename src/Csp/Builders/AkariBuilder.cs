using Csp.Objects.Constraints.Impl.Akari;
using Csp.Objects.Constraints.Impl.Akari.Objects;
using Csp.Objects.Constraints.Interfaces;
using Csp.Objects.Csp;
using Csp.Objects.Domain;
using Csp.Objects.Variables.Impl;
using Csp.Objects.Variables.Interfaces;

namespace Csp.Builders;

public class AkariBuilder
{
    private readonly List<IGridCellVariable> _variables = [];
    private readonly List<IConstraint<int>> _constraints = [];
    private readonly Domain<int> _domain = new ([0, 1]);

    private int _width = 0;
    private int _height = 0;
    private char[,] _grid;
    
    private AkariBuilder()
    {
        
    }

    public static AkariBuilder Create() => new();

    public AkariBuilder FromGrid(List<string> lines)
    {
        if (lines.Count == 0 || lines.Any(l => l.Length != lines[0].Length))
            throw new ArgumentOutOfRangeException(nameof(lines), "Invalid grid");
        
        _height = lines.Count;
        _width = lines[0].Length;

        _grid = new char[_height, _width];

        for (var y = 0; y < _height; y++)
        {
            var line = lines[y];
            for (var x = 0; x < _width; x++)
            {
                _grid[y, x] = line[x];
            }
        }

        return this;
    }

    public UniformDomainCsp<int> Build()
    {
        // turn our cells into constraints.
        // 1. create variable for every empty cell
        // 2. iterate rows ->
        //     a. for each row segment of cells, create a RowSegment
        //     b. when creating a row segment, create a noDupes constraint
        //     c. for each numbered wall, create a neighborCount constraint
        // 3. iterate columns ->
        //     a. for each column segment of walls, create a ColumnSegment
        //     b. when creating a column segment, create a noDupes constraint
        //     c. numbered walls were already covered
        // 4. iterate all cells again:
        //     a. for each empty cell, add an isLit constraint using the row and col segment that contain it

        // empty cell variables
        for (var i = 0; i < _height; i++)
        {
            for (var j = 0; j < _width; j++)
            {
                if (_grid[i, j] == '.')
                {
                    _variables.Add(new GridCellVariable(j, i));
                }
            }
        }
        
        // first pass => row segments, numbered walls
        var rowSegments = new List<RowSegment>();
        var colSegments = new List<ColumnSegment>();
        for (var y = 0; y < _height; y++)
        {
            List<IGridCellVariable> currentSegment = [];
            for (var x = 0; x < _width; x++)
            {
                switch (_grid[y, x])
                {
                    case '.': // empty cell
                        var myVar = _variables.Find(v => v.X == x && v.Y == y);
                        if (myVar == null) throw new Exception("Invalid grid");
                        currentSegment.Add(myVar);
                        break;
                    case '0' or '1' or '2' or '3' or '4': // numbered wall
                        var neighbors = GetNeighbors(x, y);
                        _constraints.Add(
                            new NeighborCountConstraint(neighbors, x, y, int.Parse(_grid[y, x].ToString())));
                        // fall through, because numbered walls are still walls.
                        goto case 'w';
                    case 'w': // blank wall, commit row segment
                        if (currentSegment.Count > 0)
                        {
                            var newSegment = new RowSegment([..currentSegment]);
                            rowSegments.Add(newSegment);
                            currentSegment = [];
                            _constraints.Add(new NoDuplicateLightsConstraint(newSegment));
                        }
                        break;
                }
            }
            // if we're still working on a segment, it has just ended
            if (currentSegment.Count > 0)
            {
                var newSegment = new RowSegment([..currentSegment]);
                rowSegments.Add(newSegment);
                _constraints.Add(new NoDuplicateLightsConstraint(newSegment));
            }
        }
        
        // second pass => column segments
        for (var x = 0; x < _width; x++)
        {
            var currentSegment = new List<IGridCellVariable>();
            for (var y = 0; y < _height; y++)
            {
                switch (_grid[y, x])
                {
                    case '.': // empty cell
                        var myVar = _variables.Find(v => v.X == x && v.Y == y);
                        if (myVar == null) throw new Exception("Invalid grid");
                        currentSegment.Add(myVar);
                        break;
                    case 'w' or '0' or '1' or '2' or '3' or '4' when currentSegment.Count > 0: // blank wall, commit col segment
                        var newSegment = new ColumnSegment([..currentSegment]);
                        colSegments.Add(newSegment);
                        currentSegment = [];
                        _constraints.Add(new NoDuplicateLightsConstraint(newSegment));
                        break;
                }
            }
            // if we're still working on a segment, it has just ended
            if (currentSegment.Count > 0)
            {
                var newSegment = new ColumnSegment([..currentSegment]);
                colSegments.Add(newSegment);
                _constraints.Add(new NoDuplicateLightsConstraint(newSegment));
            }
        }
        
        // third pass => cell is lit constraints
        for (var y = 0; y < _height; y++)
        {
            for (var x = 0; x < _width; x++)
            {
                if (_grid[y, x] != '.') continue;

                var me = _variables.Find(v => v.X == x && v.Y == y);
                if (me == null)
                    throw new Exception("Invalid grid");
                
                var row = rowSegments.Find(s => s.Contains(me));
                var column = colSegments.Find(s => s.Contains(me));
                if (row == null || column == null)
                    throw new Exception("Invalid grid");

                _constraints.Add(new CellIsLitConstraint(row, column, me));
            }
        }

        return new UniformDomainCsp<int>(_variables, _domain, _constraints);
    }

    private List<IGridCellVariable> GetNeighbors(int x, int y)
    {
        var coordsList = new List<(int x, int y)>
        {
            (x - 1, y), // west
            (x + 1, y), // east
            (x, y - 1), // north
            (x, y + 1), // south
        };
        return coordsList.Select(pair => _variables.Find(v => v.X == pair.x && v.Y == pair.y)).Where(v => v != null)
            .Select(v => v!).ToList();
    }
}