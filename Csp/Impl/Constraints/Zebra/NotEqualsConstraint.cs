using Csp.Interfaces;

namespace Csp.Impl.Constraints.Zebra;

public class NotEqualsConstraint(IVariable v1, IVariable v2) : IConstraint<int>
{
    public string Name => "NotEqual";
    public string Description => $"{v1.Name} != {v2.Name}";
    
    public IReadOnlyList<IVariable> Scope { get; } = [v1, v2];
    
    // only impossible if the other is forced to be
    public bool IsSatisfiable(IVariable v, int val, IDictionary<IVariable, IDomain<int>> domains) =>
        v == v1 ? !DomainForcesVal(domains[v2], val) : !DomainForcesVal(domains[v1], val);

    private static bool DomainForcesVal(IDomain<int> domain, int val) =>
        domain.Values.Count == 1 && domain.Values.Contains(val);
}