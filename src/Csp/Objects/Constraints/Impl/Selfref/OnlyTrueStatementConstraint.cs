using Csp.Objects.Domain;
using Csp.Objects.Variables.Interfaces;

namespace Csp.Impl.Constraints.Selfref;

public class OnlyTrueStatementConstraint(
    IOrderedVariable owner,
    IEnumerable<IOrderedVariable> scope,
    IEnumerable<BaseSelfRefConstraint> choiceList)
    : BaseSelfRefConstraint<BaseSelfRefConstraint>(scope, choiceList)
{
    public override string Name => "OnlyTrueStatement";
    public override string Description => "Only true statement";

    protected override bool IsSatisfiableInternal(IDictionary<IOrderedVariable, IDomain<string>> domains)
    {
        var candidates = domains[owner].Values.ToList();
        return candidates.Any(c => GetChoice(c).IsSatisfiable(null, null, domains));
    }
}