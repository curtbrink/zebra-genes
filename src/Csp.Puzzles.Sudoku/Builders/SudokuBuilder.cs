using System.Collections.ObjectModel;
using Csp.Core.Models.Models.Constraint.Interfaces;
using Csp.Core.Models.Models.Csp;
using Csp.Core.Models.Models.Csp.Interfaces;
using Csp.Core.Models.Models.Domain;
using Csp.Core.Models.Models.Domain.Interfaces;
using Csp.Core.Models.Models.Variable;
using Csp.Core.Models.Models.Variable.Interfaces;
using Csp.Puzzles.Sudoku.Constraints;

namespace Csp.Puzzles.Sudoku.Builders;

public class SudokuBuilder
{
    private readonly List<IGridCellVariable> _variables = [];
    private readonly List<IConstraint<int>> _constraints = [];
    private readonly ImmutableDomain<int> _domain = new(1, 2, 3, 4, 5, 6, 7, 8, 9);

    private readonly Dictionary<IGridCellVariable, int> _setCells = new();

    private SudokuBuilder()
    {
        for (var i = 0; i < 9; i++)
        {
            for (var j = 0; j < 9; j++)
            {
                _variables.Add(new GridCellVariable(j, i));
            }
        }

        // rows
        for (var i = 0; i < 9; i++)
        {
            // rows
            _constraints.Add(new UniqueConstraint(_variables.Where(v => v.Y == i).ToList(), $"Row {i + 1}"));
            // columns
            _constraints.Add(new UniqueConstraint(_variables.Where(v => v.X == i).ToList(), $"Col {i + 1}"));
            // squares
            var yStart = i / 3 * 3;
            var xStart = i % 3 * 3;
            _constraints.Add(new UniqueConstraint(_variables.Where(v =>
                    v.Y >= yStart && v.Y <= yStart + 2 && v.X >= xStart && v.X <= xStart + 2).ToList(),
                $"Square {i + 1}"));
        }
    }

    public static SudokuBuilder Create() => new();

    public SudokuBuilder FromGrid(List<string> lines)
    {
        if (lines.Count != 9 || lines.Any(l => l.Length != 9))
        {
            throw new ArgumentOutOfRangeException(nameof(lines), "Grid must be 9x9");
        }

        for (var i = 0; i < 9; i++)
        {
            for (var j = 0; j < 9; j++)
            {
                // simply ignore non-numbers
                if (int.TryParse(lines[i][j].ToString(), out var v) && v > 0)
                {
                    // set cell is meant to be 1-indexed
                    SetCell(j + 1, i + 1, v);
                }
            }
        }

        return this;
    }

    public SudokuBuilder SetCell(int x, int y, int v)
    {
        if (!_domain.Values.Contains(v))
        {
            throw new ArgumentOutOfRangeException(nameof(v), "Value must be 1-9 inclusive");
        }

        // add to our "reference guide"
        var variable = _variables.Find(gridVar => gridVar.X == x - 1 && gridVar.Y == y - 1);
        if (variable == null)
        {
            throw new ArgumentOutOfRangeException(nameof(x), "x and y values must be 1-9 inclusive");
        }

        // we simply set that variable's domain at build time
        var success = _setCells.TryAdd(variable, v);
        if (!success)
        {
            throw new ArgumentOutOfRangeException(nameof(x), $"A value for {x},{y} was already set");
        }

        return this;
    }

    public ICsp<int> Build()
    {
        // turn our set cells into real domains
        var dict = new Dictionary<IVariable, IDomain<int>>();
        foreach (var v in _variables)
        {
            if (_setCells.TryGetValue(v, out var cell))
            {
                dict[v] = new ImmutableDomain<int>(cell);
            }
            else
            {
                dict[v] = new ImmutableDomain<int>(_domain.Values);
            }
        }

        return new BaseCsp<int>(_variables, new ReadOnlyDictionary<IVariable, IDomain<int>>(dict), _constraints);
    }
}