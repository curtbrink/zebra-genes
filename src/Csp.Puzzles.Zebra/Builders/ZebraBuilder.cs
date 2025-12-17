using Csp.Core.Models.Models.Constraint.Interfaces;
using Csp.Core.Models.Models.Csp;
using Csp.Core.Models.Models.Csp.Interfaces;
using Csp.Core.Models.Models.Domain;
using Csp.Core.Models.Models.Variable;
using Csp.Core.Models.Models.Variable.Interfaces;
using Csp.Puzzles.Zebra.Constraints;

namespace Csp.Puzzles.Zebra.Builders;

public partial class ZebraBuilder
{
    private readonly int _size;
    private readonly Dictionary<string, List<string>> _categories = new();

    private readonly List<IVariable> _variables = [];
    private readonly List<IConstraint<int>> _constraints = [];
    private readonly ImmutableDomain<int> _domain;

    private ZebraBuilder(int size)
    {
        _size = size;
        _domain = BuildDomain(size);
    }

    public static ZebraBuilder Create(int size) => new(size);

    public ZebraBuilder AddCategory(string name, List<string> values)
    {
        if (values.Count != _size)
        {
            throw new ArgumentOutOfRangeException(nameof(values),
                $"Value list size {values.Count} must equal puzzle size {_size}");
        }

        // add to our "reference guide"
        _categories.Add(name, values);

        // make CSP variables
        var newVariables = values.Select(v => new BaseVariable(BuildKey(name, v))).ToList();
        _variables.AddRange(newVariables);

        // all values in category must be in a different position
        _constraints.Add(new AllDifferentConstraint(newVariables, name));

        return this;
    }

    public ZebraConstraintBuilder AddConstraint(string name)
    {
        var variable = GetVariable(name);
        return new ZebraConstraintBuilder(this, variable);
    }

    public (ICsp<int>, Dictionary<string, List<string>>) Build() =>
        (new BaseCsp<int>(_variables, _domain, _constraints), _categories);

    private IVariable GetVariable(string name)
    {
        var categoryName = _categories.Keys.FirstOrDefault(k => _categories[k].Contains(name));
        if (categoryName == null)
        {
            throw new KeyNotFoundException($"Category containing {name} not found");
        }

        var varKey = BuildKey(categoryName, name);
        return _variables.First(v => v.Name == varKey);
    }

    private static ImmutableDomain<int> BuildDomain(int size)
    {
        var d = new List<int>();
        for (var i = 1; i <= size; i++)
        {
            d.Add(i);
        }

        return new ImmutableDomain<int>(d);
    }

    private static string BuildKey(string categoryName, string categoryValue) => $"{categoryName}:{categoryValue}";
}