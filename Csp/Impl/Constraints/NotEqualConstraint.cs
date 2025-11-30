using Csp.Interfaces;

namespace Csp.Impl.Constraints;

// this constraint ensures two variables have different solutions
public class NotEqualConstraint(IVariable v1, IVariable v2) : IConstraint<string>
{
    public string Name => "NotEqual";
    public string Description => $"{v1.Name} != {v2.Name}";
    public IReadOnlyList<IVariable> Scope { get; } = [v1, v2];

    public bool IsSatisfiable(IVariable v, string val, IDictionary<IVariable, IDomain<string>> domains)
    {
        return v == v1
            ? domains[v2].Values.Any(otherVal => otherVal != val)
            : domains[v1].Values.Any(otherVal => otherVal != val);
    }
}