using Csp.Objects.Constraints.Interfaces;
using Csp.Objects.Domain;
using Csp.Objects.Variables.Interfaces;

namespace Csp.Objects.Csp;

public class Csp<T>(IReadOnlyCollection<IVariable> variables, IReadOnlyDictionary<IVariable, IDomain<T>> domains,
    IReadOnlyCollection<IConstraint<T>> constraints) : ICsp<T>
{
    public IReadOnlyCollection<IVariable> Variables { get; } = variables;
    public IReadOnlyDictionary<IVariable, IDomain<T>> Domains { get; } = domains;
    public IReadOnlyCollection<IConstraint<T>> Constraints { get; } = constraints;
}