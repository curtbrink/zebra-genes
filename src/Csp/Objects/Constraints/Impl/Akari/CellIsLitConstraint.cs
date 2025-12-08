using Csp.Objects.Constraints.Impl.Akari.Objects;
using Csp.Objects.Domain;
using Csp.Objects.Variables.Interfaces;

namespace Csp.Objects.Constraints.Impl.Akari;

public class CellIsLitConstraint(RowSegment row, ColumnSegment column, IGridCellVariable owner) : BaseAkariConstraint
{
    public override string Name => $"{owner.Name} is lit";
    public override string Description => $"{owner.Name} is light or lit by row/column";

    public override IReadOnlyList<IVariable> Scope =>
        new List<IGridCellVariable>([..row.Cells, ..column.Cells]).Distinct().ToList();

    protected override bool IsSatisfiableInternal(IDictionary<IGridCellVariable, IDomain<int>> domains)
    {
        // am I or can I be a light?
        var myDomain = domains[owner].Values;
        if (myDomain.Contains(1)) return true;
        
        // can I be lit by my row or column?
        return Scope.Where(sv => sv != owner).Select(sv => (IGridCellVariable)sv).Select(sv => domains[sv].Values)
            .Any(svd => svd.Contains(1));
    }
}