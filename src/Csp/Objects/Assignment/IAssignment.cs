using Csp.Objects.Variables.Interfaces;

namespace Csp.Objects.Assignment;

public interface IAssignment<T>
{
    bool IsAssigned(IVariable v);
    T GetValue(IVariable v);
}