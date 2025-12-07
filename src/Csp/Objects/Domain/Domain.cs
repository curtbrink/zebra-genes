namespace Csp.Objects.Domain;

public class Domain<T>(IEnumerable<T> values) : IDomain<T>
{
    public ISet<T> Values { get; } = new HashSet<T>(values);
}