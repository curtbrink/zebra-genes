using Csp.Interfaces;

namespace Csp.Impl.Constraints.Selfref;

public class ChoiceEqualsConstraint(IVariable owner, IVariable lookingAt, IEnumerable<string> options)
    : BaseSelfRefConstraint<string>([owner, lookingAt], options)
{
    public override string Name => "ChoiceEquals";
    public override string Description => $"{owner.Name}=>{lookingAt.Name} is {{{OptionString}}}";

    public override bool IsSatisfiable(IVariable v, string val, IDictionary<IVariable, IDomain<string>> domains)
    {
        // owner's choice is val
        if (v == owner)
        {
            var lookingAtMustBe = ChoiceList[val];
            return domains[lookingAt].Values.Contains(lookingAtMustBe);
        }
        // lookingAt's choice is val
        var ownerMustBe = ChoiceList.Keys.FirstOrDefault(k => ChoiceList[k] == val);
        return ownerMustBe != null && domains[owner].Values.Contains(ownerMustBe);
    }
}