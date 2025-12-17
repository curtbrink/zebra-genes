using Csp.Core.Models.Models.Domain.Interfaces;
using Csp.Core.Models.Models.Variable.Interfaces;

namespace Csp.Puzzles.Selfref.Constraints;

public class ChoiceEqualsConstraint(
    IOrderedVariable owner,
    IOrderedVariable lookingAt,
    IReadOnlyCollection<string> options)
    : BaseSelfRefConstraint<string>([owner, lookingAt], options)
{
    public override string Name => "ChoiceEquals";
    public override string Description => $"{owner.Name}=>{lookingAt.Name} is {{{OptionString}}}";

    public override bool IsSatisfiable(IDomainStore<string> domains) => domains.GetDomain(owner).Values
        .Select(GetChoice).Any(otherVal => domains.GetDomain(lookingAt).Values.Contains(otherVal));
}