namespace Csp.Interfaces;

public interface IConstraint<T>
{
    string Name { get; }
    string Description { get; }
    IReadOnlyList<IVariable> Scope { get; }
    
    bool IsSatisfiable(IVariable v, T val, IDictionary<IVariable, IDomain<T>> domains);
}