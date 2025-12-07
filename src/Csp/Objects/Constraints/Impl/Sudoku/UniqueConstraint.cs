using Csp.Objects.Domain;
using Csp.Objects.Variables.Interfaces;

namespace Csp.Objects.Constraints.Impl.Sudoku;

public class UniqueConstraint(IReadOnlyList<IGridCellVariable> scope, string name) : BaseSudokuConstraint
{
    public override string Name => name;
    public override string Description => $"{name} uniqueness";
    public override IReadOnlyList<IVariable> Scope => scope;
    protected override bool IsSatisfiableInternal(IDictionary<IGridCellVariable, IDomain<int>> domains)
    {
        // can't work if any domain is empty
        var scopeDomains = scope.Select(sv => domains[sv]).ToList();
        if (scopeDomains.Any(dv => dv.Values.Count == 0)) return false;

        var forcedValues = scopeDomains.Where(dv => dv.Values.Count == 1).Select(dv => dv.Values.First()).ToList();

        // can't work if a forced value is duped
        if (forcedValues.Count != forcedValues.Distinct().Count()) return false;

        // nothing obvious
        return true;
    }
}