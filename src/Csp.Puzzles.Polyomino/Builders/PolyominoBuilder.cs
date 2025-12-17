using System.Collections.ObjectModel;
using Csp.Core.Models.Models.Csp;
using Csp.Core.Models.Models.Csp.Interfaces;
using Csp.Core.Models.Models.Domain;
using Csp.Core.Models.Models.Domain.Interfaces;
using Csp.Core.Models.Models.Variable.Interfaces;
using Csp.Puzzles.Polyomino.Constraints;
using Csp.Puzzles.Polyomino.Models;

namespace Csp.Puzzles.Polyomino.Builders;

public class PolyominoBuilder
{
    private readonly List<Models.Polyomino> _polyominos;
    private readonly Dictionary<Models.Polyomino, int> _polyominoQuotas;
    private readonly int _gridWidth;
    private readonly int _gridHeight;

    private PolyominoBuilder(int w, int h)
    {
        _gridWidth = w;
        _gridHeight = h;
        _polyominoQuotas = new Dictionary<Models.Polyomino, int>();
        _polyominos = [];
    }

    public static PolyominoBuilder Create(int w, int h) => new(w, h);

    public PolyominoBuilder AddPolyomino(Models.Polyomino p, int count)
    {
        _polyominoQuotas[p] = count;
        _polyominos.Add(p);
        return this;
    }

    public ICsp<Placement> Build()
    {
        var variables = new List<PolyominoVariable>();
        var domains = new Dictionary<Models.Polyomino, List<Placement>>();
        foreach (var p in _polyominos)
        {
            for (var i = 0; i < _polyominoQuotas[p]; i++)
            {
                variables.Add(new PolyominoVariable($"{i}", p));
            }

            // find domain
            domains[p] = [];

            for (var i = 0; i < p.PossiblePlacements.Count; i++)
            {
                // find footprint for placement
                var pl = p.PossiblePlacements[i];
                var xCoords = pl.Select(c => c.x).ToList();
                var yCoords = pl.Select(c => c.y).ToList();
                var minX = xCoords.Min(); // should always be 0
                if (minX > 0) throw new Exception("weird non-normalized footprint");
                var maxX = xCoords.Max();
                var minY = yCoords.Min(); // should always be 0
                if (minY > 0) throw new Exception("weird non-normalized footprint");
                var maxY = yCoords.Max();

                // if grid x axis is 0..9 (10-width)
                // and footprint is 3x3 (0..2 both sides)
                // then placement could be placed up to 7,7 (7..9, 7..9 each side)

                var gridMaxX = _gridWidth - maxX;
                var gridMaxY = _gridHeight - maxY;

                for (var x = 0; x < gridMaxX; x++)
                {
                    for (var y = 0; y < gridMaxY; y++)
                    {
                        domains[p].Add(new Placement(x, y, i, p));
                    }
                }
            }

            Console.WriteLine($"There are {domains[p].Count} domain options for {p.Id}");
        }

        var builtDomains = new Dictionary<IVariable, IDomain<Placement>>();
        foreach (var pv in variables)
        {
            var listCopy = domains[pv.P].ToList();
            builtDomains[pv] = new ImmutableDomain<Placement>(listCopy);
        }

        var readonlyDict = new ReadOnlyDictionary<IVariable, IDomain<Placement>>(builtDomains);

        return new BaseCsp<Placement>(variables, readonlyDict, [new NoOverlapConstraint(variables)]);
    }
}