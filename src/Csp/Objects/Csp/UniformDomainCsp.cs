using System.Collections.ObjectModel;
using Csp.Objects.Constraints.Interfaces;
using Csp.Objects.Domain;
using Csp.Objects.Variables.Interfaces;

namespace Csp.Objects.Csp;

public class UniformDomainCsp<T> : ICsp<T>
{
    public IReadOnlyCollection<IVariable> Variables { get; }
    public IReadOnlyDictionary<IVariable, IDomain<T>> Domains { get; }
    public IReadOnlyCollection<IConstraint<T>> Constraints { get; }

    public UniformDomainCsp(IReadOnlyCollection<IVariable> variables, IDomain<T> domain,
        IReadOnlyCollection<IConstraint<T>> constraints)
    {
        Variables = variables;
        Constraints = constraints;
        
        var d = new Dictionary<IVariable, IDomain<T>>();
        foreach (var v in variables)
        {
            d[v] = new Domain<T>(domain.Values.ToList()); // copy the domain
        }

        Domains = new ReadOnlyDictionary<IVariable, IDomain<T>>(d);
    }
}