using Csp.Core.Models.Models.Domain.Interfaces;
using Csp.Core.Models.Models.Variable.Interfaces;

namespace Csp.Puzzles.Selfref.Constraints;

public class OnlySameChoiceConstraint(
    IOrderedVariable owner,
    IReadOnlyCollection<IOrderedVariable> scope,
    IReadOnlyCollection<int> choiceList)
    : BaseSelfRefConstraint<int>(scope, choiceList)
{
    public override string Name => "OnlySameChoice";
    public override string Description => $"Only same answer as {owner.Name} is {owner.Name}={{{OptionString}}}";

    public override bool IsSatisfiable(IDomainStore<string> domains)
    {
        // for each option ABCDE in owner's domain, it is supported if:
        // - Q(choices[j]) has j in its domain, AND
        // - no other Q outside of Q(choices[j]) and owner have j forcibly assigned.

        var possibleValues = domains.GetDomain(owner).Values.ToList();
        foreach (var candidate in possibleValues)
        {
            // get target question
            var target = scope.FirstOrDefault(s => s.Id == GetChoice(candidate));
            if (target == null) continue;

            if (!domains.GetDomain(target).Values.Contains(candidate)) continue;

            var allOtherQuestions = scope.Where(s => s != target && s != owner).ToList();
            var isValid = allOtherQuestions.Select(q => domains.GetDomain(q).Values)
                .All(qd => qd.Count > 1 || !qd.Contains(candidate));

            if (isValid) return true;
        }

        return false;
    }
}