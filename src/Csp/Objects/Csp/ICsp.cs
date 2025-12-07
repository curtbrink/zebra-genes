using Csp.Objects.Constraints.Interfaces;
using Csp.Objects.Domain;
using Csp.Objects.Variables.Interfaces;

namespace Csp.Objects.Csp;

public interface ICsp<T>
{
    IReadOnlyCollection<IVariable> Variables { get; }
    IReadOnlyDictionary<IVariable, IDomain<T>> Domains { get; }
    IReadOnlyCollection<IConstraint<T>> Constraints { get; }
}