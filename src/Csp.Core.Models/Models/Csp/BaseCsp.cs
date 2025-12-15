using Csp.Core.Models.Models.Constraint.Interfaces;
using Csp.Core.Models.Models.Csp.Interfaces;
using Csp.Core.Models.Models.Domain;
using Csp.Core.Models.Models.Domain.Interfaces;
using Csp.Core.Models.Models.Variable.Interfaces;

namespace Csp.Core.Models.Models.Csp;

public class BaseCsp<T> : ICsp<T>
{
    public IReadOnlyCollection<IVariable> Variables { get; }
    public IReadOnlyDictionary<IVariable, IDomain<T>> Domains { get; }
    public IReadOnlyCollection<IConstraint<T>> Constraints { get; }

    private BaseCsp(IReadOnlyCollection<IVariable> variables,
        IReadOnlyCollection<IConstraint<T>> constraints)
    {
        Variables = variables;
        Constraints = constraints;
        Domains = new Dictionary<IVariable, IDomain<T>>();
    }
    
    public BaseCsp(IReadOnlyCollection<IVariable> variables, IReadOnlyDictionary<IVariable, IDomain<T>> domains,
        IReadOnlyCollection<IConstraint<T>> constraints) : this(variables, constraints)
    {
        Domains = domains;
    }
    
    public BaseCsp(IReadOnlyCollection<IVariable> variables, IDomain<T> domain,
        IReadOnlyCollection<IConstraint<T>> constraints) : this(variables, constraints)
    {
        var d = new Dictionary<IVariable, IDomain<T>>();
        foreach (var v in variables)
        {
            d[v] = new ImmutableDomain<T>(domain.Values.ToList()); // copy the domain
        }

        Domains = d;
    }
}