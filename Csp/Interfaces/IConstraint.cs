namespace Csp.Interfaces;

public interface IConstraint<T>
{
    string Name { get; }
    string Description { get; }
    IReadOnlyList<IVariable> Scope { get; }
    
    bool IsSatisfied(IAssignment<T> assignment);
}