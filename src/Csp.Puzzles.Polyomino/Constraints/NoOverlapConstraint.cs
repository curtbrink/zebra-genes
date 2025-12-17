using Csp.Core.Models.Models.Constraint.Interfaces;
using Csp.Core.Models.Models.Domain.Interfaces;
using Csp.Core.Models.Models.Variable.Interfaces;
using Csp.Puzzles.Polyomino.Models;

namespace Csp.Puzzles.Polyomino.Constraints;

public class NoOverlapConstraint(List<PolyominoVariable> scope) : IConstraint<Placement>
{
    public string Name => "NoOverlap";
    public string Description => "No overlapping polyomino placements";
    public IReadOnlyList<IVariable> Scope => scope;

    public bool IsSatisfiable(IDomainStore<Placement> domains)
    {
        var forcedPlacements = domains.GetAllDomains()
            .Select(kvp => (kvp.Key, domains.GetDomain(kvp.Key).Values))
            .Where(kvp => kvp.Values.Count == 1).ToList();

        if (forcedPlacements.Count < 2) return true;

        for (var i = 0; i < forcedPlacements.Count; i++)
        {
            var pi = forcedPlacements[i].Values.First();
            for (var j = i + 1; j < forcedPlacements.Count; j++)
            {
                var pj = forcedPlacements[j].Values.First();

                // check if pi and pj overlap
                var piCoords = pi.P.PossiblePlacements[pi.Variation].Select(c => (c.x + pi.X, c.y + pi.Y)).ToList();
                var pjCoords = pj.P.PossiblePlacements[pj.Variation].Select(c => (c.x + pj.X, c.y + pj.Y)).ToList();

                if (piCoords.Any(c => pjCoords.Contains(c)) || pjCoords.Any(c => piCoords.Contains(c))) return false;
            }
        }

        return true;
    }
}