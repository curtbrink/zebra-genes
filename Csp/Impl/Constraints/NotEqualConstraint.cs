using Csp.Interfaces;

namespace CspTests.ZebraTest;

// this constraint ensures two variables have different solutions
public class NotEqualConstraint(IVariable v1, IVariable v2) : IConstraint<string>
{
    public string Name => "NotEqual";
    public string Description => $"{v1.Name} != {v2.Name}";
    public IReadOnlyList<IVariable> Scope { get; } = [v1, v2];

    public bool IsSatisfied(IAssignment<string> assignment)
    {
        if (!assignment.IsAssigned(v1) || !assignment.IsAssigned(v2))
        {
            return false;
        }

        return assignment.GetValue(v1) != assignment.GetValue(v2);
    }

    public bool ReduceDomains(ICsp<string> csp, IDictionary<IVariable, ISet<string>> domains)
    {
        throw new NotImplementedException();
    }
}