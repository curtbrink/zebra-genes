using Csp.Objects.Constraints.Interfaces;
using Csp.Objects.Domain;
using Csp.Objects.Variables.Interfaces;

namespace Csp.Objects.Constraints.Impl.Zebra;

public class EqualsConstraint(IVariable v1, IVariable v2) : IConstraint<int>
{
    public string Name => "Equal";
    public string Description => $"{v1.Name} == {v2.Name}";
    public IReadOnlyList<IVariable> Scope { get; } = [v1, v2];

    public bool IsSatisfiable(IVariable? v, int val, IDictionary<IVariable, IDomain<int>> domains) =>
        v == v1 ? domains[v2].Values.Contains(val) : domains[v1].Values.Contains(val);
}