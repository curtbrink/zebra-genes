namespace Csp.Core.Models.Models.Domain.Interfaces;

public interface IDomain<T>
{
    IReadOnlySet<T> Values { get; }
}