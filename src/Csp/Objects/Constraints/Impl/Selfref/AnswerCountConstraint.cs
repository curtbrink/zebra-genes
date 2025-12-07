using Csp.Objects.Domain;
using Csp.Objects.Variables.Interfaces;

namespace Csp.Objects.Constraints.Impl.Selfref;

public class AnswerCountConstraint(
    IOrderedVariable owner,
    IEnumerable<IOrderedVariable> scope,
    IEnumerable<string> toCount,
    IEnumerable<int> answers)
    : BaseSelfRefConstraint<int>(scope.Contains(owner) ? scope : [..scope, owner], answers)
{
    public override string Name => "CountOf";
    public override string Description => $"count({toCount}) is {owner.Name}=>{{{OptionString}}}";

    protected override bool IsSatisfiableInternal(IDictionary<IOrderedVariable, IDomain<string>> domains)
    {
        // unlike most/least common which is much more complex, we just want to know if count(j) can be hit.
        var toCountPool = toCount.ToList();
        var optionsToCheck = domains[owner].Values.ToList();
        var min = 0;
        var max = 0;
        foreach (var question in scope)
        {
            // if checking AE, are any values in the domain A or E?
            if (domains[question].Values.Any(dv => toCountPool.Contains(dv)))
            {
                max++;
                if (domains[question].Values.Count == 1)
                {
                    // assigned!
                    min++;
                }
            }
        }

        return optionsToCheck.Any(o => GetChoice(o) >= min && GetChoice(o) <= max);
    }
}