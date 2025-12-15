using Csp.Core.Models.Models.Constraint.Interfaces;
using Csp.Core.Models.Models.Domain.Interfaces;
using Csp.Core.Models.Models.Variable.Interfaces;
using Csp.Puzzles.Akari.Models;

namespace Csp.Puzzles.Akari.Constraints;

public class NoDuplicateLightsConstraint(Segment segment) : IConstraint<int>
{
    public string Name => segment.Name;
    public string Description => "<= 1 light in segment";
    public IReadOnlyList<IVariable> Scope => [..segment.Cells];

    public bool IsSatisfiable(IDomainStore<int> domains) => Scope.Select(domains.GetDomain)
        .Count(svd => svd.Values.Count == 1 && svd.Values.First() == 1) <= 1;
}