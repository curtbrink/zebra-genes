using Csp.Interfaces;

namespace Csp.Impl.Constraints.Selfref;

public class ChoiceEqualsConstraint(IVariable owner, IVariable lookingAt, IEnumerable<string> options)
    : BaseSelfRefConstraint<string>([owner, lookingAt], options)
{
    public override string Name => "ChoiceEquals";
    public override string Description => $"{owner.Name}=>{lookingAt.Name} is {{{OptionString}}}";

    public override bool IsSatisfied(IAssignment<string> assignment)
    {
        if (!assignment.IsAssigned(owner) || !assignment.IsAssigned(lookingAt))
        {
            return false;
        }

        return ChoiceList[assignment.GetValue(owner)] == assignment.GetValue(lookingAt);
    }
}