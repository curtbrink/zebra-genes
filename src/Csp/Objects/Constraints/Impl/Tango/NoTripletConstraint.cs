using Csp.Objects.Domain;
using Csp.Objects.Variables.Interfaces;

namespace Csp.Objects.Constraints.Impl.Tango;

public class NoTripletConstraint(IReadOnlyList<IGridCellVariable> scope) : BaseTangoConstraint
{
    public override string Name => "NoTriplet";
    public override string Description => "Triplet has at most 2 of the same";
    public override IReadOnlyList<IVariable> Scope => scope;
    
    protected override bool IsSatisfiableInternal(IDictionary<IGridCellVariable, IDomain<int>> domains)
    {
        var d = scope.Select(sv => domains[sv].Values).ToList();
        if (d.Any(dv => dv.Count > 1)) return true;
        return d.Select(dv => dv.First()).Distinct().Count() > 1;
    }
}