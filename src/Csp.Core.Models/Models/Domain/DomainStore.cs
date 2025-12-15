using Csp.Core.Models.Models.Domain.Interfaces;
using Csp.Core.Models.Models.Variable.Interfaces;

namespace Csp.Core.Models.Models.Domain;

public class DomainStore<T>(IDictionary<IVariable, IDomain<T>> domains) : IDomainStore<T>
{
    public IDomain<T> GetDomain(IVariable variable) => domains[variable];
}