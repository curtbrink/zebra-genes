namespace Csp.Interfaces;

public interface IAssignment<T>
{
    bool IsAssigned(IVariable v);
    T GetValue(IVariable v);
}