using Csp.Core.Models.Models.Constraint.Interfaces;
using Csp.Core.Models.Models.Csp.Interfaces;
using Csp.Core.Models.Models.Domain.Interfaces;
using Csp.Core.Models.Models.Variable.Interfaces;

namespace Csp.Core.Models.Models.Csp;

public class BaseCsp<T>(IReadOnlyCollection<IVariable> variables, IReadOnlyDictionary<IVariable, IDomain<T>> domains,
    IReadOnlyCollection<IConstraint<T>> constraints) : ICsp<T>
{
    public IReadOnlyCollection<IVariable> Variables { get; } = variables;
    public IReadOnlyDictionary<IVariable, IDomain<T>> Domains { get; } = domains;
    public IReadOnlyCollection<IConstraint<T>> Constraints { get; } = constraints;
}