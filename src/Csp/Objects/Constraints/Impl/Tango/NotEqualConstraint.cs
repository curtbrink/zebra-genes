using Csp.Objects.Domain;
using Csp.Objects.Variables.Interfaces;

namespace Csp.Objects.Constraints.Impl.Tango;

public class NotEqualConstraint(IGridCellVariable one, IGridCellVariable two) : BaseTangoConstraint
{
    public override string Name => "NotEqual";
    public override string Description => "Two adjacent cells are different";
    public override IReadOnlyList<IVariable> Scope => [one, two];
    
    protected override bool IsSatisfiableInternal(IDictionary<IGridCellVariable, IDomain<int>> domains)
    {
        return Scope.Select(sv => domains[(IGridCellVariable)sv].Values).Any(dv => dv.Count > 1) ||
               domains[one].Values.First() != domains[two].Values.First();
    }
}