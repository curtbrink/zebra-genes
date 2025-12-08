using Csp.Objects.Constraints.Impl.Akari.Objects;
using Csp.Objects.Domain;
using Csp.Objects.Variables.Interfaces;

namespace Csp.Objects.Constraints.Impl.Akari;

public class NoDuplicateLightsConstraint(Segment segment) : BaseAkariConstraint
{
    public override string Name => segment.Name;
    public override string Description => "<= 1 light in segment";
    public override IReadOnlyList<IVariable> Scope => [..segment.Cells];

    protected override bool IsSatisfiableInternal(IDictionary<IGridCellVariable, IDomain<int>> domains) =>
        Scope.Select(sv => domains[(IGridCellVariable)sv].Values).Count(svd => svd.Count == 1 && svd.First() == 1) <= 1;
}