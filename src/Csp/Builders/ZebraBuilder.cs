using Csp.Impl;
using Csp.Impl.Constraints.Zebra;
using Csp.Interfaces;
using Csp.Objects.Domain;
using Csp.Objects.Variables.Impl;
using Csp.Objects.Variables.Interfaces;

namespace Csp.Builders;

public class ZebraBuilder
{
    private readonly int _size;
    private readonly Dictionary<string, List<string>> _categories = new();

    private readonly List<IVariable> _variables = [];
    private readonly List<IConstraint<int>> _constraints = [];
    private readonly Domain<int> _domain;
    
    private ZebraBuilder(int size)
    {
        _size = size;
        _domain = BuildDomain(size);
    }

    public static ZebraBuilder Create(int size) => new (size);

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

    public (UniformDomainCsp<int>, Dictionary<string, List<string>>) Build() =>
        (new UniformDomainCsp<int>(_variables, _domain, _constraints), _categories);

    protected IVariable GetVariable(string name)
    {
        var categoryName = _categories.Keys.FirstOrDefault(k => _categories[k].Contains(name));
        if (categoryName == null)
        {
            throw new KeyNotFoundException($"Category containing {name} not found");
        }

        var varKey = BuildKey(categoryName, name);
        return _variables.First(v => v.Name == varKey);
    }

    private static Domain<int> BuildDomain(int size)
    {
        var d = new List<int>();
        for (var i = 1; i <= size; i++)
        {
            d.Add(i);
        }

        return new Domain<int>(d);
    }

    private static string BuildKey(string categoryName, string categoryValue) => $"{categoryName}:{categoryValue}";

    public class ZebraConstraintBuilder
    {
        private ZebraBuilder _builder;
        private IVariable _primary;

        internal ZebraConstraintBuilder(ZebraBuilder builder, IVariable primary)
        {
            _builder = builder;
            _primary = primary;
        }

        public ZebraBuilder MustBeInPosition(int pos) => MustBeInPosition([pos]);

        public ZebraBuilder MustBeInPosition(List<int> pos)
        {
            var constraint = new OneOfConstraint(_primary, pos);
            return AddAndClose(constraint);
        }

        public ZebraBuilder Has(string other) => Is(other);

        public ZebraBuilder Is(string other)
        {
            var otherVar = _builder.GetVariable(other);
            var constraint = new EqualsConstraint(_primary, otherVar);
            return AddAndClose(constraint);
        }

        public ZebraBuilder IsAdjacentTo(string other)
        {
            var otherVar = _builder.GetVariable(other);
            var constraint = new AdjacentConstraint(_primary, otherVar);
            return AddAndClose(constraint);
        }

        public ZebraBuilder IsBefore(string other) => IsBefore(other, false);

        public ZebraBuilder IsAfter(string other) => IsBefore(other, true);

        private ZebraBuilder IsBefore(string other, bool isAfter)
        {
            var otherVar = _builder.GetVariable(other);
            var before = isAfter ? otherVar : _primary;
            var after = isAfter ? _primary : otherVar;
            var constraint = new BeforeConstraint(before, after);
            return AddAndClose(constraint);
        }

        public ZebraBuilder IsImmediatelyBefore(string other) => IsNBefore(other, 1);

        public ZebraBuilder IsImmediatelyAfter(string other) => IsNAfter(other, 1);
        
        public ZebraBuilder IsNBefore(string other, int pos) => IsNBefore(other, pos, false);
        
        public ZebraBuilder IsNAfter(string other, int pos) => IsNBefore(other, pos, true);

        private ZebraBuilder IsNBefore(string other, int n, bool reverse)
        {
            var otherVar = _builder.GetVariable(other);
            var before = reverse ? otherVar : _primary;
            var after = reverse ? _primary : otherVar;
            var constraint = new OffsetConstraint(before, after, n);
            return AddAndClose(constraint);
        }

        private ZebraBuilder AddAndClose(IConstraint<int> constraint)
        {
            if (CheckForDuplicates(constraint))
            {
                var conflictingConstraint = CheckForConflicts(constraint);
                if (conflictingConstraint != null)
                {
                    throw new Exception(
                        $"Constraint conflicts with existing constraint \"{conflictingConstraint.Description}\"");
                }
                _builder._constraints.Add(constraint);
            }

            return _builder;
        }

        private bool CheckForDuplicates(IConstraint<int> newConstraint)
        {
            // todo this would be useful for idempotency!
            return true;
        }

        private IConstraint<int>? CheckForConflicts(IConstraint<int> newConstraint)
        {
            // todo catch low-hanging logical fruit such as:
            // - equals on the same category
            // - same OneOfs on the same category
            // - etc
            return null;
        }
    }
    
}