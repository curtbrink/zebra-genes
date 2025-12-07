using Csp.Objects.Domain;
using Csp.Objects.Variables.Interfaces;

namespace Csp.Objects.Constraints.Impl.Selfref;

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
            var otherOption = GetChoice(c);
            if (domains[lookingAt].Values.Contains(otherOption)) return true;
        }

        return false;
    }
}