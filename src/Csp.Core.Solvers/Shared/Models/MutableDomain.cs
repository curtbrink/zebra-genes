using Csp.Core.Models.Models.Domain.Interfaces;
using Csp.Core.Solvers.Shared.Models.Interfaces;

namespace Csp.Core.Solvers.Shared.Models;

public class MutableDomain<T> : IMutableDomain<T>
{
    public IReadOnlySet<T> Values => _values;
    
    private HashSet<T> _values;
    
    public MutableDomain(IEnumerable<T> values)
    {
        _values = new HashSet<T>(values);
    }
    
    public MutableDomain(params T[] values)
    {
        _values = new HashSet<T>(values);
    }

    public MutableDomain(IDomain<T> existing)
    {
        _values = new HashSet<T>(existing.Values);
    }

    public void Remove(T item)
    {
        _values.Remove(item);
    }

    public void SetTo(T item)
    {
        _values = new HashSet<T>([item]);
    }

    public void SetTo(IEnumerable<T> items)
    {
        _values = new HashSet<T>(items);
    }
}