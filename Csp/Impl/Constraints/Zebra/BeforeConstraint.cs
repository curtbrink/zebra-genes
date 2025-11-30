using Csp.Interfaces;

namespace Csp.Impl.Constraints.Zebra;

public class BeforeConstraint(IVariable v1, IVariable v2) : IConstraint<int>
{
    public string Name => "Before";
    public string Description => $"{v1.Name} is before {v2.Name}";
    
    public IReadOnlyList<IVariable> Scope { get; } = [v1, v2];

    public bool IsSatisfiable(IVariable v, int val, IDictionary<IVariable, IDomain<int>> domains) =>
        v == v1 ? domains[v2].Values.Any(p => p > val) : domains[v1].Values.Any(p => p < val);
}