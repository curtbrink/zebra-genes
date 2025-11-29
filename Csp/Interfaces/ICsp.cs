namespace Csp.Interfaces;

public interface ICsp<T>
{
    IReadOnlyCollection<IVariable> Variables { get; }
    IReadOnlyDictionary<IVariable, IDomain<T>> Domains { get; }
    IReadOnlyCollection<IConstraint<T>> Constraints { get; }
}