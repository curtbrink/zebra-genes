using Csp.Helpers;
using Csp.Interfaces;

namespace Csp.Impl.Constraints.Selfref;

public abstract class BaseSelfRefConstraint : IConstraint<string>
{
    protected readonly IList<string> Options = ["A", "B", "C", "D", "E"];
    
    public abstract string Name { get; }
    public abstract string Description { get; }
    
    public abstract IReadOnlyList<IVariable> Scope { get; }

    public bool IsSatisfiable(IVariable? v, string? val, IDictionary<IOrderedVariable, IDomain<string>> domains)
    {
        var newDomains = DeepCopy.ToOrderedDomains(domains);
        return IsSatisfiableWithNewDomains(v, val, newDomains);
    }
    
    public bool IsSatisfiable(IVariable? v, string? val, IDictionary<IVariable, IDomain<string>> domains)
    {
        var newDomains = DeepCopy.ToOrderedDomains(domains);
        return IsSatisfiableWithNewDomains(v, val, newDomains);
    }

    private bool IsSatisfiableWithNewDomains(IVariable? v, string? val,
        IDictionary<IOrderedVariable, IDomain<string>> domains)
    {
        if (v is not null && val is not null)
        {
            if (v is not IOrderedVariable orderedV)
                throw new ArgumentOutOfRangeException(nameof(v), "Owner variable must be ordered variable");
            domains[orderedV] = new Domain<string>([val]);
        }

        return IsSatisfiableInternal(domains);
    }

    protected abstract bool IsSatisfiableInternal(IDictionary<IOrderedVariable, IDomain<string>> domains);
}