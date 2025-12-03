using Csp.Interfaces;

namespace Csp.Impl.Constraints.Zebra;

public class AdjacentConstraint(IVariable v1, IVariable v2) : IConstraint<int>
{
    public string Name => "Adjacent";
    public string Description => $"{v1.Name} is next to {v2.Name}";
    
    public IReadOnlyList<IVariable> Scope { get; } = [v1, v2];
    
    public bool IsSatisfiable(IVariable? v, int val, IDictionary<IVariable, IDomain<int>> domains)
    {
        var other = v == v1 ? v2 : v1;
        return domains[other].Values.Any(otherV => otherV == val - 1 || otherV == val + 1);
    }
}