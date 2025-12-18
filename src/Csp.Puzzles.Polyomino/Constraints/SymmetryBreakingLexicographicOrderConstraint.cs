using Csp.Core.Models.Models.Constraint.Interfaces;
using Csp.Core.Models.Models.Domain.Interfaces;
using Csp.Core.Models.Models.Variable.Interfaces;
using Csp.Puzzles.Polyomino.Models;

namespace Csp.Puzzles.Polyomino.Constraints;

public class SymmetryBreakingLexicographicOrderConstraint(PolyominoVariable a, PolyominoVariable b) : IConstraint<Placement>
{
    public string Name => "LexicographicOrder";
    public string Description => "Lexicographic order symmetry breaking";
    public IReadOnlyList<IVariable> Scope => [a, b];
    public bool IsSatisfiable(IDomainStore<Placement> domains)
    {
        var aDomain = domains.GetDomain(a).Values;
        var bDomain = domains.GetDomain(b).Values;

        var aForced = aDomain.Count == 1;
        var bForced = bDomain.Count == 1;

        // Both forced
        if (aForced && bForced)
            return aDomain.First().Compare(bDomain.First()) <= 0;

        // A forced, B not
        if (aForced)
            return bDomain.Any(bVal => aDomain.First().Compare(bVal) <= 0);

        // B forced, A not
        if (bForced)
            return aDomain.Any(aVal => aVal.Compare(bDomain.First()) <= 0);

        // Neither forced → always satisfiable
        return true;
    }
}