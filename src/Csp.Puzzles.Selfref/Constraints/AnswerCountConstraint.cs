using Csp.Core.Models.Models.Domain.Interfaces;
using Csp.Core.Models.Models.Variable.Interfaces;

namespace Csp.Puzzles.Selfref.Constraints;

public class AnswerCountConstraint(
    IOrderedVariable owner,
    IReadOnlyCollection<IOrderedVariable> scope,
    IReadOnlyCollection<string> toCount,
    IReadOnlyCollection<int> answers)
    : BaseSelfRefConstraint<int>(scope.Contains(owner) ? scope : [..scope, owner], answers)
{
    public override string Name => "CountOf";
    public override string Description => $"count({toCount}) is {owner.Name}=>{{{OptionString}}}";

    public override bool IsSatisfiable(IDomainStore<string> domains)
    {
        // unlike most/least common which is much more complex, we just want to know if count(j) can be hit.
        var toCountPool = toCount.ToList();
        var optionsToCheck = domains.GetDomain(owner).Values.ToList();
        var min = 0;
        var max = 0;
        foreach (var question in scope)
        {
            // if checking AE, are any values in the domain A or E?
            var domainVals = domains.GetDomain(question).Values;
            if (!domainVals.Any(dv => toCountPool.Contains(dv))) continue;
            
            max++;
            if (domainVals.Count == 1)
            {
                // assigned!
                min++;
            }
        }

        return optionsToCheck.Any(o => GetChoice(o) >= min && GetChoice(o) <= max);
    }
}