using Csp.Core.Solvers.Shared.Models.Interfaces;

namespace Csp.Core.Solvers.Shared.Interfaces;

public interface ISearchState<T>
{
    public IMutableDomainStore<T>? DomainStore { get; set; }
}