using Csp.Core.Models.Models.Constraint.Interfaces;
using Csp.Core.Models.Models.Domain.Interfaces;
using Csp.Core.Models.Models.Variable.Interfaces;

namespace Csp.Puzzles.Zebra.Constraints;

public class OneOfConstraint(IVariable owner, List<int> allowedValues) : IConstraint<int>
{
    public string Name => _isSingleton ? "Is" : "IsOneOf";

    public string Description => _isSingleton
        ? $"{owner.Name} is {{{allowedValues[0]}}}"
        : $"{owner.Name} is one of {{{string.Join(",", allowedValues)}}}";
    
    public IReadOnlyList<IVariable> Scope { get; } = [owner];

    private bool _isSingleton = allowedValues.Count == 1;
    
    public bool IsSatisfiable(IDomainStore<int> domains) => domains.GetDomain(owner).Values.Any(allowedValues.Contains);
}