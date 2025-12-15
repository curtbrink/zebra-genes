using Csp.Core.Models.Models.Csp.Interfaces;
using Csp.Core.Solvers.Shared.Models.Interfaces;

namespace Csp.Core.Solvers.Shared.Interfaces;

public interface IInferenceSolver<T> : ISolver<T>
{
    public void Propagate(ICsp<T> csp, IMutableDomainStore<T> domains);
}