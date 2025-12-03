using Csp.Interfaces;

namespace Csp.Impl.Constraints.Selfref;

public class ChoiceEqualsConstraint(IOrderedVariable owner, IOrderedVariable lookingAt, IEnumerable<string> options)
    : BaseSelfRefConstraint<string>([owner, lookingAt], options)
{
    public override string Name => "ChoiceEquals";
    public override string Description => $"{owner.Name}=>{lookingAt.Name} is {{{OptionString}}}";

    protected override bool IsSatisfiableInternal(IDictionary<IOrderedVariable, IDomain<string>> domains)
    {
        var candidates = domains[owner].Values.ToList();
        
        foreach (var c in candidates)
        {
            var otherOption = ChoiceList[c];
            if (domains[lookingAt].Values.Contains(otherOption)) return true;
        }

        return false;
    }
}