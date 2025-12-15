using Csp.Core.Models.Models.Domain.Interfaces;

namespace Csp.Core.Solvers.Shared.Models.Interfaces;

public interface IMutableDomain<T> : IDomain<T>
{
    public void Remove(T item);
    public void SetTo(T item);
    public void SetTo(IEnumerable<T> items);
}