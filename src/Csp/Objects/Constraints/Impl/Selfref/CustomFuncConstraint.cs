using Csp.Objects.Domain;
using Csp.Objects.Variables.Interfaces;

namespace Csp.Objects.Constraints.Impl.Selfref;

public class CustomFuncConstraint(
    IOrderedVariable owner,
    IEnumerable<IOrderedVariable> scope,
    Func<IOrderedVariable,IDictionary<IOrderedVariable, IDomain<string>>,bool> isPossibleFunc)
    : BaseSelfRefConstraint<string?>(scope, new List<string?>())
{
    public override string Name => "CustomConstraint";

    public override string Description => "Custom logic constraint";

    // isPossibleFunc should return TRUE if ANY full assignment of the domains can satisfy the constraint.
    protected override bool IsSatisfiableInternal(IDictionary<IOrderedVariable, IDomain<string>> domains) =>
        isPossibleFunc(owner, domains);
}