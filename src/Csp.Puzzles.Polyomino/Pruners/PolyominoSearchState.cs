using Csp.Core.Solvers.Shared.Interfaces;
using Csp.Core.Solvers.Shared.Models.Interfaces;
using Csp.Puzzles.Polyomino.Models;

namespace Csp.Puzzles.Polyomino.Pruners;

public class PolyominoSearchState : ISearchState<Placement>
{
    public int GridWidth { get; init; }
    public int GridHeight { get; init; }

    public required List<Models.Polyomino> Pieces { get; init; }
    
    public required Dictionary<Models.Polyomino, int> Quotas { get; init; }
    
    public IMutableDomainStore<Placement>? DomainStore { get; set; }
}