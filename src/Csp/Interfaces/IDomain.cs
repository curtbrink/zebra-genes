namespace Csp.Interfaces;

public interface IDomain<T>
{
    ISet<T> Values { get; }
}