using Csp.Core.Models.Models.Domain.Interfaces;
using Csp.Core.Models.Models.Variable.Interfaces;

namespace Csp.Puzzles.Selfref.Constraints;

public class CustomFuncConstraint(
    IOrderedVariable owner,
    IReadOnlyCollection<IOrderedVariable> scope,
    Func<IOrderedVariable, IDomainStore<string>, bool> isPossibleFunc)
    : BaseSelfRefConstraint<string?>(scope, new List<string?>())
{
    public override string Name => "CustomConstraint";

    public override string Description => "Custom logic constraint";

    // isPossibleFunc should return TRUE if ANY full assignment of the domains can satisfy the constraint.
    public override bool IsSatisfiable(IDomainStore<string> domains) => isPossibleFunc(owner, domains);
}