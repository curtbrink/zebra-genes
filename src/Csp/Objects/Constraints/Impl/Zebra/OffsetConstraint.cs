using Csp.Objects.Constraints.Interfaces;
using Csp.Objects.Domain;
using Csp.Objects.Variables.Interfaces;

namespace Csp.Objects.Constraints.Impl.Zebra;

public class OffsetConstraint(IVariable v1, IVariable v2, int offset = 1) : IConstraint<int>
{
    public string Name => "Offset";

    public string Description => offset == 1
        ? $"{v1.Name} immediately left of {v2.Name}"
        : $"{v1.Name} {offset} positions left of {v2.Name}";

    public IReadOnlyList<IVariable> Scope { get; } = [v1, v2];

    public bool IsSatisfiable(IVariable? v, int val, IDictionary<IVariable, IDomain<int>> domains) =>
        v == v1 ? domains[v2].Values.Contains(val + offset) : domains[v1].Values.Contains(val - offset);
}