using Csp.Core.Models.Models.Constraint.Interfaces;
using Csp.Core.Models.Models.Domain.Interfaces;
using Csp.Core.Models.Models.Variable.Interfaces;

namespace Csp.Puzzles.Zebra.Constraints;

public class AllDifferentConstraint(IReadOnlyList<IVariable> scope, string category) : IConstraint<int>
{
    public string Name => $"AllDifferent({category})";
    public string Description => $"Uniqueness for {category}";

    public IReadOnlyList<IVariable> Scope { get; } = scope;
    
    public bool IsSatisfiable(IDomainStore<int> domains)
    {
        var allDomainsWithForcedDomains = Scope.Select(domains.GetDomain).Where(sd => sd.Values.Count == 1).ToList();

        for (var i = 0; i < allDomainsWithForcedDomains.Count; i++)
        {
            for (var j = i + 1; j < allDomainsWithForcedDomains.Count; j++)
            {
                if (allDomainsWithForcedDomains[i].Values.First() == allDomainsWithForcedDomains[j].Values.First())
                    return false;
            }
        }

        return true;
    }

    public bool IsSatisfiable(IVariable? v, int val, IDictionary<IVariable, IDomain<int>> domains)
    {
        // impossible if any other variable in v's category is forced to val
        return Scope.Where(v2 => v2 != v).Where(v2 => domains[v2].Values.Count <= 1)
            .All(v2 => !domains[v2].Values.Contains(val));
    }
}