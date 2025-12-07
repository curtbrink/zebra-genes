using Csp.Objects.Domain;
using Csp.Objects.Variables.Interfaces;

namespace Csp.Objects.Constraints.Impl.Selfref;

public class OnlySameChoiceConstraint(
    IOrderedVariable owner,
    IEnumerable<IOrderedVariable> scope,
    IEnumerable<int> choiceList)
    : BaseSelfRefConstraint<int>(scope, choiceList)
{
    public override string Name => "OnlySameChoice";
    public override string Description => $"Only same answer as {owner.Name} is {owner.Name}={{{OptionString}}}";
    
    protected override bool IsSatisfiableInternal(IDictionary<IOrderedVariable, IDomain<string>> domains)
    {
        // for each option ABCDE in owner's domain, it is supported if:
        // - Q(choices[j]) has j in its domain, AND
        // - no other Q outside of Q(choices[j]) and owner have j forcibly assigned.

        var possibleValues = domains[owner].Values.ToList();
        foreach (var candidate in possibleValues)
        {
            // get target question
            var target = scope.FirstOrDefault(s => s.Id == GetChoice(candidate));
            if (target == null) continue;

            if (!domains[target].Values.Contains(candidate)) continue;

            var allOtherQuestions = scope.Where(s => s != target && s != owner).ToList();
            var isValid =
                allOtherQuestions.All(q => domains[q].Values.Count > 1 || !domains[q].Values.Contains(candidate));

            if (isValid) return true;
        }

        return false;
    }
}