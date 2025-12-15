using Csp.Core.Models.Models.Domain.Interfaces;
using Csp.Core.Models.Models.Variable.Interfaces;

namespace Csp.Core.Solvers.Shared.Models.Interfaces;

public interface IMutableDomainStore<T> : IDomainStore<T>
{
    public IMutableDomain<T> GetMutableDomain(IVariable variable);
    
    public void SetMutableDomain(IVariable variable, IMutableDomain<T> domain);

    public IDictionary<IVariable, IMutableDomain<T>> GetAllDomains();
}