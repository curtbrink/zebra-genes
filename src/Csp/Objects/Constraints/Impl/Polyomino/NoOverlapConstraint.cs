using Csp.Objects.Constraints.Interfaces;
using Csp.Objects.Domain;
using Csp.Objects.Variables.Impl;
using Csp.Objects.Variables.Interfaces;
using Csp.Types.Polyomino;

namespace Csp.Objects.Constraints.Impl.Polyomino;

public class NoOverlapConstraint(List<PolyominoVariable> scope) : IConstraint<Placement>
{
    public string Name => "NoOverlap";
    public string Description => "No overlapping polyomino placements";
    public IReadOnlyList<IVariable> Scope => scope;
    public bool IsSatisfiable(IVariable? v, Placement? val, IDictionary<IVariable, IDomain<Placement>> domains)
    {
        var forcedPlacements = domains.ToList()
            .Where(kv => kv.Key == v || (kv.Key != v && domains[kv.Key].Values.Count == 1))
            .Select(kv => kv.Key == v ? (kv.Key, new Domain<Placement>([val!])) : (kv.Key, kv.Value)).ToList();

        if (forcedPlacements.Count < 2) return true;

        for (var i = 0; i < forcedPlacements.Count; i++)
        {
            var pi = forcedPlacements[i].Value.Values.First();
            for (var j = i + 1; j < forcedPlacements.Count; j++)
            {
                var pj = forcedPlacements[j].Value.Values.First();
                
                // check if pi and pj overlap
                var piCoords = pi.P.PossiblePlacements[pi.Variation].Select(c => (c.x + pi.X, c.y + pi.Y)).ToList();
                var pjCoords = pj.P.PossiblePlacements[pj.Variation].Select(c => (c.x + pj.X, c.y + pj.Y)).ToList();

                if (piCoords.Any(c => pjCoords.Contains(c)) || pjCoords.Any(c => piCoords.Contains(c))) return false;
            }
        }

        return true;
    }
}