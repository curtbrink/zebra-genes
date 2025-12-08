using Csp.Objects.Domain;
using Csp.Objects.Variables.Interfaces;

namespace Csp.Objects.Constraints.Impl.Akari;

public class NeighborCountConstraint(IReadOnlyList<IGridCellVariable> scope, int x, int y, int count) : BaseAkariConstraint
{
    public override string Name => $"{x},{y}";
    public override string Description => $"{Name} lights={count}";
    public override IReadOnlyList<IVariable> Scope => scope;
    protected override bool IsSatisfiableInternal(IDictionary<IGridCellVariable, IDomain<int>> domains)
    {
        var scopeDomains = scope.Select(sv => domains[sv].Values).ToList();
        var minLights = scopeDomains.Count(sd => sd.Count == 1 && sd.First() == 1);
        var maxLights = scopeDomains.Count(sd => sd.Any(sdv => sdv == 1));

        return count >= minLights && count <= maxLights;
    }
}