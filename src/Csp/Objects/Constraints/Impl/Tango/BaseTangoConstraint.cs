using Csp.Helpers;
using Csp.Objects.Constraints.Interfaces;
using Csp.Objects.Domain;
using Csp.Objects.Variables.Interfaces;

namespace Csp.Objects.Constraints.Impl.Tango;

public abstract class BaseTangoConstraint : IConstraint<int>
{
    protected List<int> TangoValues { get; } = [1, 2];
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract IReadOnlyList<IVariable> Scope { get; }
    public bool IsSatisfiable(IVariable? v, int val, IDictionary<IVariable, IDomain<int>> domains)
    {
        var newDomains = DeepCopy.DowncastDomains<int, IGridCellVariable>(domains);
        
        if (v is not null && TangoValues.Contains(val))
        {
            if (v is not IGridCellVariable gridV)
                throw new ArgumentOutOfRangeException(nameof(v),
                    "Partial assignment variable must be grid cell variable");
            newDomains[gridV] = new Domain<int>([val]);
        }

        return IsSatisfiableInternal(newDomains);
    }

    protected abstract bool IsSatisfiableInternal(IDictionary<IGridCellVariable, IDomain<int>> domains);
}