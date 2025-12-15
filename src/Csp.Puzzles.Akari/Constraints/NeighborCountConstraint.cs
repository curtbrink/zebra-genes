using Csp.Core.Models.Models.Constraint.Interfaces;
using Csp.Core.Models.Models.Domain.Interfaces;
using Csp.Core.Models.Models.Variable.Interfaces;

namespace Csp.Puzzles.Akari.Constraints;

public class NeighborCountConstraint(IReadOnlyList<IGridCellVariable> scope, int x, int y, int count) : IConstraint<int>
{
    public string Name => $"{x},{y}";
    public string Description => $"{Name} lights={count}";
    public IReadOnlyList<IVariable> Scope => scope;

    public bool IsSatisfiable(IDomainStore<int> domains)
    {
        var scopeDomains = Scope.Select(domains.GetDomain).ToList();
        var minLights = scopeDomains.Count(sd => sd.Values.Count == 1 && sd.Values.First() == 1);
        var maxLights = scopeDomains.Count(sd => sd.Values.Any(sdv => sdv == 1));

        return count >= minLights && count <= maxLights;
    }
}