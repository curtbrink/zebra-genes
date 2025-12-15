using Csp.Core.Models.Models.Constraint.Interfaces;
using Csp.Core.Models.Models.Domain.Interfaces;
using Csp.Core.Models.Models.Variable.Interfaces;
using Csp.Puzzles.Akari.Models;

namespace Csp.Puzzles.Akari.Constraints;

public class CellIsLitConstraint(RowSegment row, ColumnSegment column, IGridCellVariable owner) : IConstraint<int>
{
    public string Name => $"{owner.Name} is lit";
    public string Description => $"{owner.Name} is light or lit by row/column";

    public IReadOnlyList<IVariable> Scope =>
        new List<IGridCellVariable>([..row.Cells, ..column.Cells]).Distinct().ToList();

    // satisfiable if either I or any of my row/column mates can be lit
    public bool IsSatisfiable(IDomainStore<int> domains) => domains.GetDomain(owner).Values.Contains(1) ||
                                                            Scope.Where(sv => sv != owner).Select(domains.GetDomain)
                                                                .Any(svd => svd.Values.Contains(1));
}