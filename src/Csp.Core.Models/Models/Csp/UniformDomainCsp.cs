using System.Collections.ObjectModel;
using Csp.Core.Models.Models.Constraint.Interfaces;
using Csp.Core.Models.Models.Csp.Interfaces;
using Csp.Core.Models.Models.Domain;
using Csp.Core.Models.Models.Domain.Interfaces;
using Csp.Core.Models.Models.Variable.Interfaces;

namespace Csp.Core.Models.Models.Csp;

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