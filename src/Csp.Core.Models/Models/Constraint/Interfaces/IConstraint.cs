using Csp.Core.Models.Models.Domain.Interfaces;
using Csp.Core.Models.Models.Variable.Interfaces;

namespace Csp.Core.Models.Models.Constraint.Interfaces;

public interface IConstraint<T>
{
    string Name { get; }
    string Description { get; }
    IReadOnlyList<IVariable> Scope { get; }
    
    bool IsSatisfiable(IDomainStore<T> domains);
}