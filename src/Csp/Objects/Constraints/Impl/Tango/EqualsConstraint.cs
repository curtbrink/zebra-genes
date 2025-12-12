using Csp.Objects.Domain;
using Csp.Objects.Variables.Interfaces;

namespace Csp.Objects.Constraints.Impl.Tango;

public class EqualsConstraint(IGridCellVariable one, IGridCellVariable two) : BaseTangoConstraint
{
    public override string Name => "Equals";
    public override string Description => "Two adjacent cells equal";
    public override IReadOnlyList<IVariable> Scope => [one, two];
    
    protected override bool IsSatisfiableInternal(IDictionary<IGridCellVariable, IDomain<int>> domains)
    {
        return domains[one].Values.Any(v => domains[two].Values.Contains(v)) &&
               domains[two].Values.Any(v => domains[one].Values.Contains(v));
    }
}