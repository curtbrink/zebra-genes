using Csp.Interfaces;

namespace Csp.Impl.Constraints.Selfref;

public class AnswerCountConstraint(
    IVariable owner,
    IEnumerable<IVariable> scope,
    string toCount,
    IEnumerable<int> answers)
    : BaseSelfRefConstraint<int>(scope, answers)
{
    public override string Name => "CountOf";
    public override string Description => $"count({toCount}) is {owner.Name}=>{{{OptionString}}}";

    public override bool IsSatisfiable(IVariable v, string val, IDictionary<IVariable, IDomain<string>> domains)
    {
        // unlike most/least common which is much more complex, we just want to know if count(j) can be hit.
        var optionsToCheck = v == owner ? [val] : domains[owner].Values.ToList();
        var min = 0;
        var max = 0;
        foreach (var question in Scope)
        {
            if (question == v)
            {
                if (val == toCount)
                {
                    min++;
                    max++;
                }

                continue;
            }

            if (domains[question].Values.Contains(toCount))
            {
                max++;
                if (domains[question].Values.Count == 1)
                {
                    // assigned!
                    min++;
                }
            }
        }

        return optionsToCheck.Any(o => ChoiceList[o] >= min && ChoiceList[o] <= max);
    }
}