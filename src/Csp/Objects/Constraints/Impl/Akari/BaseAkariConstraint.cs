using Csp.Helpers;
using Csp.Objects.Constraints.Interfaces;
using Csp.Objects.Domain;
using Csp.Objects.Variables.Interfaces;

namespace Csp.Objects.Constraints.Impl.Akari;

public abstract class BaseAkariConstraint : IConstraint<int>
{
    protected List<int> AkariValues { get; } = [0, 1];
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract IReadOnlyList<IVariable> Scope { get; }
    public bool IsSatisfiable(IVariable? v, int val, IDictionary<IVariable, IDomain<int>> domains)
    {
        var newDomains = DeepCopy.DowncastDomains<int, IGridCellVariable>(domains);
        
        if (v is not null && AkariValues.Contains(val))
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