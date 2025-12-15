using Csp.Core.Models.Models.Constraint.Interfaces;
using Csp.Core.Models.Models.Domain.Interfaces;
using Csp.Core.Models.Models.Variable.Interfaces;

namespace Csp.Puzzles.Zebra.Constraints;

public class OffsetConstraint(IVariable v1, IVariable v2, int offset = 1) : IConstraint<int>
{
    public string Name => "Offset";

    public string Description => offset == 1
        ? $"{v1.Name} immediately left of {v2.Name}"
        : $"{v1.Name} {offset} positions left of {v2.Name}";

    public IReadOnlyList<IVariable> Scope { get; } = [v1, v2];

    public bool IsSatisfiable(IDomainStore<int> domains) => domains.GetDomain(v1).Values
        .Any(v1V => domains.GetDomain(v2).Values.Any(v2V => v1V == v2V - offset));
}