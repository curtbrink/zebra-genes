using Csp.Core.Models.Models.Domain.Interfaces;

namespace Csp.Core.Models.Models.Domain;

public class ImmutableDomain<T> : IDomain<T>
{
    public ImmutableDomain(IEnumerable<T> values)
    {
        Values = new HashSet<T>(values);
    }
    
    public ImmutableDomain(params T[] values)
    {
        Values = new HashSet<T>(values);
    }

    public ImmutableDomain(IDomain<T> existing)
    {
        Values = new HashSet<T>(existing.Values);
    }
    
    public IReadOnlySet<T> Values { get; }
}