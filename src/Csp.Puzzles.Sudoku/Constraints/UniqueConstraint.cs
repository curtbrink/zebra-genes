using Csp.Core.Models.Models.Constraint.Interfaces;
using Csp.Core.Models.Models.Domain.Interfaces;
using Csp.Core.Models.Models.Variable.Interfaces;

namespace Csp.Puzzles.Sudoku.Constraints;

public class UniqueConstraint(IReadOnlyList<IGridCellVariable> scope, string name) : IConstraint<int>
{
    public string Name => name;
    public string Description => $"{name} uniqueness";
    public IReadOnlyList<IVariable> Scope => scope;

    public bool IsSatisfiable(IDomainStore<int> domains)
    {
        // can't work if any domain is empty
        var scopeDomains = Scope.Select(domains.GetDomain).ToList();
        if (scopeDomains.Any(dv => dv.Values.Count == 0)) return false;

        var forcedValues = scopeDomains.Where(dv => dv.Values.Count == 1).Select(dv => dv.Values.First()).ToList();

        // can't work if a forced value is duped. otherwise no obvious contradictions here
        return forcedValues.Count == forcedValues.Distinct().Count();
    }
}