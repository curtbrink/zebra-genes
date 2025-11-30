using Csp.Interfaces;

namespace Csp.Impl.Constraints;

public class EqualsConstraint(IVariable owner, string expected) : IConstraint<string>
{
    public string Name => "Equals";
    public string Description => $"{owner.Name} == \"{expected}\"";
    public IReadOnlyList<IVariable> Scope { get; } = [owner];

    public bool IsSatisfiable(IVariable v, string val, IDictionary<IVariable, IDomain<string>> domains) =>
        v == owner ? val == expected : domains[v].Values.Contains(expected);
}