using Csp.Core.Models.Models.Constraint.Interfaces;
using Csp.Core.Models.Models.Domain.Interfaces;
using Csp.Core.Models.Models.Variable.Interfaces;

namespace Csp.Core.Models.Models.Csp.Interfaces;

public interface ICsp<T>
{
    IReadOnlyCollection<IVariable> Variables { get; }
    IReadOnlyDictionary<IVariable, IDomain<T>> Domains { get; }
    IReadOnlyCollection<IConstraint<T>> Constraints { get; }
}