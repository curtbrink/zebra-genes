using Csp.Core.Models.Models.Variable.Interfaces;

namespace Csp.Core.Models.Models.Domain.Interfaces;

public interface IDomainStore<T>
{
    public IDomain<T> GetDomain(IVariable variable);
    
    public IDictionary<IVariable, IDomain<T>> GetAllDomains();

}