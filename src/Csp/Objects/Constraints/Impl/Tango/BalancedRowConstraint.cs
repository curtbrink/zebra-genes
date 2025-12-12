using Csp.Objects.Domain;
using Csp.Objects.Variables.Interfaces;

namespace Csp.Objects.Constraints.Impl.Tango;

public class BalancedRowConstraint(IReadOnlyList<IGridCellVariable> scope, string name) : BaseTangoConstraint
{
    public override string Name => name;
    public override string Description => $"{name} balanced";
    public override IReadOnlyList<IVariable> Scope => scope;
    
    protected override bool IsSatisfiableInternal(IDictionary<IGridCellVariable, IDomain<int>> domains)
    {
        // tango requires 3 + 3 in each row and column
        // can't work if any domain is empty
        var scopeDomains = scope.Select(sv => domains[sv]).ToList();
        if (scopeDomains.Any(dv => dv.Values.Count == 0)) return false;

        var minA = 0;
        var minB = 0;
        var maxA = 0;
        var maxB = 0;
        foreach (var sv in scopeDomains)
        {
            if (sv.Values.Count == 0) return false;

            if (sv.Values.Count == 1)
            {
                switch (sv.Values.First())
                {
                    case 1:
                        minA++;
                        break;
                    case 2:
                        minB++;
                        break;
                }
            }

            if (sv.Values.Contains(1)) maxA++;
            if (sv.Values.Contains(2)) maxB++;
        }

        return minA <= 3 && maxA >= 3 && minB <= 3 && maxB >= 3;
    }
}