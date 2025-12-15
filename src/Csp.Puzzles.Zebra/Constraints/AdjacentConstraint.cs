using Csp.Core.Models.Models.Constraint.Interfaces;
using Csp.Core.Models.Models.Domain.Interfaces;
using Csp.Core.Models.Models.Variable.Interfaces;

namespace Csp.Puzzles.Zebra.Constraints;

public class AdjacentConstraint(IVariable v1, IVariable v2) : IConstraint<int>
{
    public string Name => "Adjacent";
    public string Description => $"{v1.Name} is next to {v2.Name}";
    
    public IReadOnlyList<IVariable> Scope { get; } = [v1, v2];

    public bool IsSatisfiable(IDomainStore<int> domains) => domains.GetDomain(v1).Values
        .Any(v1V => domains.GetDomain(v2).Values.Any(v2V => Math.Abs(v1V - v2V) == 1));
}