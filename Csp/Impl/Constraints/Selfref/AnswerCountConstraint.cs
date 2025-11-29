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

    public override bool IsSatisfied(IAssignment<string> assignment)
    {
        if (!assignment.IsAssigned(owner))
        {
            return false;
        }

        var ownerChoice = assignment.GetValue(owner);
        return ChoiceList[ownerChoice] ==
               Scope.Count(v => assignment.IsAssigned(v) && assignment.GetValue(v) == toCount);
    }
}