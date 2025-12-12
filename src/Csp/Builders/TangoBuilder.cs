using System.Collections.ObjectModel;
using Csp.Objects.Constraints.Impl.Tango;
using Csp.Objects.Constraints.Interfaces;
using Csp.Objects.Csp;
using Csp.Objects.Domain;
using Csp.Objects.Variables.Impl;
using Csp.Objects.Variables.Interfaces;

namespace Csp.Builders;

public class TangoBuilder
{
    private readonly List<IGridCellVariable> _variables = [];
    private readonly List<IConstraint<int>> _constraints = [];
    private readonly Domain<int> _domain = new ([1, 2]);

    private readonly int _size;

    private readonly Dictionary<IGridCellVariable, int> _setCells = new();
    
    private TangoBuilder(int size)
    {
        _size = size;
        for (var i = 0; i < _size; i++)
        {
            for (var j = 0; j < _size; j++)
            {
                _variables.Add(new GridCellVariable(j, i));
            }
        }
        
        for (var i = 0; i < size; i++)
        {
            // rows
            _constraints.Add(new BalancedRowConstraint(_variables.Where(v => v.Y == i).ToList(), $"Row {i + 1}"));
            // columns
            _constraints.Add(new BalancedRowConstraint(_variables.Where(v => v.X == i).ToList(), $"Col {i + 1}"));
            // triplets
            for (var j = 0; j < size - 2; j++)
            {
                int[] range = [j, j + 1, j + 2];
                var i1 = i;
                var rowTriple = range.Select(r => _variables.Find(v => v.Y == i1 && v.X == r)!).ToList();
                _constraints.Add(new NoTripletConstraint(rowTriple));
                var colTriple = range.Select(r => _variables.Find(v => v.X == i1 && v.Y == r)!).ToList();
                _constraints.Add(new NoTripletConstraint(colTriple));
            }
        }
    }

    public static TangoBuilder Create(int size) => new(size);

    public TangoBuilder FromGrid(List<string> lines)
    {
        if (lines.Count != _size || lines.Any(l => l.Length != _size))
        {
            throw new ArgumentOutOfRangeException(nameof(lines), $"Grid must be {_size}x{_size}");
        }

        for (var i = 0; i < _size; i++)
        {
            for (var j = 0; j < _size; j++)
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

    public TangoBuilder SetCell(int x, int y, int v)
    {
        if (!_domain.Values.Contains(v))
        {
            throw new ArgumentOutOfRangeException(nameof(v), "Value must be 1-2 inclusive");
        }
        
        // add to our "reference guide"
        var variable = _variables.Find(gridVar => gridVar.X == x - 1 && gridVar.Y == y - 1);
        if (variable == null)
        {
            throw new ArgumentOutOfRangeException(nameof(x), $"x and y values must be 1-{_size} inclusive");
        }
        
        // we simply set that variable's domain at build time
        var success = _setCells.TryAdd(variable, v);
        if (!success)
        {
            throw new ArgumentOutOfRangeException(nameof(x), $"A value for {x},{y} was already set");
        }

        return this;
    }

    public TangoBuilder AddEqualsConstraint((int x, int y) one, (int x, int y) two)
    {
        var v1 = _variables.Find(v => v.X == one.x - 1 && v.Y == one.y - 1);
        var v2 = _variables.Find(v => v.X == two.x - 1 && v.Y == two.y - 1);
        if (v1 == null ||  v2 == null) 
            throw new ArgumentOutOfRangeException(nameof(one), $"one and two values must be 1-{_size} inclusive");

        _constraints.Add(new EqualsConstraint(v1, v2));

        return this;
    }
    
    public TangoBuilder AddNotEqualConstraint((int x, int y) one, (int x, int y) two)
    {
        var v1 = _variables.Find(v => v.X == one.x - 1 && v.Y == one.y - 1);
        var v2 = _variables.Find(v => v.X == two.x - 1 && v.Y == two.y - 1);
        if (v1 == null ||  v2 == null) 
            throw new ArgumentOutOfRangeException(nameof(one), $"one and two values must be 1-{_size} inclusive");

        _constraints.Add(new NotEqualConstraint(v1, v2));

        return this;
    }

    public Csp<int> Build()
    {
        // turn our set cells into real domains
        var dict = new Dictionary<IVariable, IDomain<int>>();
        foreach (var v in _variables)
        {
            if (!_setCells.ContainsKey(v))
            {
                dict[v] = new Domain<int>(_domain.Values);
            }
            else
            {
                dict[v] = new Domain<int>([_setCells[v]]);
            }
        }

        return new Csp<int>(_variables, new ReadOnlyDictionary<IVariable, IDomain<int>>(dict), _constraints);
    }
}