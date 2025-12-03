using Csp.Interfaces;

namespace Csp.Impl;

public class Assignment<T> : IAssignment<T>
{
    private readonly Dictionary<IVariable, T> _variableAssignments;
    
    public Assignment(IVariable v, T vValue, IList<IVariable> otherVs, IList<T> otherVValues)
    {
        _variableAssignments = new Dictionary<IVariable, T>
        {
            [v] = vValue
        };
        for (var i = 0; i < otherVs.Count; i++)
        {
            _variableAssignments[otherVs[i]] = otherVValues[i];
        }
    }
    
    public bool IsAssigned(IVariable v) => _variableAssignments.ContainsKey(v);

    public T GetValue(IVariable v) => _variableAssignments[v];
}