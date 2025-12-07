using Csp.Helpers;
using Csp.Objects.Constraints.Interfaces;
using Csp.Objects.Domain;
using Csp.Objects.Variables.Interfaces;

namespace Csp.Objects.Constraints.Impl.Selfref;

public abstract class BaseSelfRefConstraint : IConstraint<string>
{
    protected readonly IList<string> Options = ["A", "B", "C", "D", "E"];
    
    public abstract string Name { get; }
    public abstract string Description { get; }
    
    public abstract IReadOnlyList<IVariable> Scope { get; }
    
    // set this flag when using as part of an OnlyTrueStatement, so that we always look in choiceList["A"].
    public bool OverrideSingle = false;

    public bool IsSatisfiable(IVariable? v, string? val, IDictionary<IOrderedVariable, IDomain<string>> domains)
    {
        var newDomains = DeepCopy.Domains(domains);
        return IsSatisfiableWithNewDomains(v, val, newDomains);
    }
    
    public bool IsSatisfiable(IVariable? v, string? val, IDictionary<IVariable, IDomain<string>> domains)
    {
        var newDomains = DeepCopy.DowncastDomains<string, IOrderedVariable>(domains);
        return IsSatisfiableWithNewDomains(v, val, newDomains);
    }

    private bool IsSatisfiableWithNewDomains(IVariable? v, string? val,
        IDictionary<IOrderedVariable, IDomain<string>> domains)
    {
        if (v is not null && val is not null)
        {
            if (v is not IOrderedVariable orderedV)
                throw new ArgumentOutOfRangeException(nameof(v), "Partial assignment variable must be ordered variable");
            domains[orderedV] = new Domain<string>([val]);
        }

        return IsSatisfiableInternal(domains);
    }

    protected abstract bool IsSatisfiableInternal(IDictionary<IOrderedVariable, IDomain<string>> domains);
}