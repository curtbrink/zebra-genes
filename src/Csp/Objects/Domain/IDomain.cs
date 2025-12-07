namespace Csp.Objects.Domain;

public interface IDomain<T>
{
    ISet<T> Values { get; }
}