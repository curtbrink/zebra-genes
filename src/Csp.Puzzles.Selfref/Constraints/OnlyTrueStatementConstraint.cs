using Csp.Core.Models.Models.Domain.Interfaces;
using Csp.Core.Models.Models.Variable.Interfaces;

namespace Csp.Puzzles.Selfref.Constraints;

public class OnlyTrueStatementConstraint(
    IOrderedVariable owner,
    IReadOnlyCollection<IOrderedVariable> scope,
    IReadOnlyCollection<BaseSelfRefConstraint> choiceList)
    : BaseSelfRefConstraint<BaseSelfRefConstraint>(scope, choiceList)
{
    public override string Name => "OnlyTrueStatement";
    public override string Description => "Only true statement";

    public override bool IsSatisfiable(IDomainStore<string> domains)
    {
        var candidates = domains.GetDomain(owner).Values.ToList();
        return candidates.Any(c => GetChoice(c).IsSatisfiable(domains));
    }
}