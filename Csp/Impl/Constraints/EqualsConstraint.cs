using Csp.Interfaces;

namespace Csp.Impl.Constraints;

public class EqualsConstraint(IVariable v, string expected) : IConstraint<string>
{
    public string Name => "Equals";
    public string Description => $"{v.Name} == \"{expected}\"";
    public IReadOnlyList<IVariable> Scope { get; } = [v];

    public bool IsSatisfied(IAssignment<string> assignment) =>
        assignment.IsAssigned(v) && assignment.GetValue(v) == expected;
}