using Csp.Core.Models.Models.Domain.Interfaces;

namespace Csp.Core.Models.Models.Domain;

public class Domain<T> : IDomain<T>
{
    public Domain(IEnumerable<T> values)
    {
        Values = new HashSet<T>(values);
    }
    
    public Domain(params T[] values)
    {
        Values = new HashSet<T>(values);
    }

    public Domain(IDomain<T> existing)
    {
        Values = new HashSet<T>(existing.Values);
    }
    
    public IReadOnlySet<T> Values { get; }
}