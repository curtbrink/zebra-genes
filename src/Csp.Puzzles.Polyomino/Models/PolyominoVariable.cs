using Csp.Core.Models.Models.Variable.Interfaces;

namespace Csp.Puzzles.Polyomino.Models;

public class PolyominoVariable(string id, Polyomino p) : IVariable
{
    public string Name => $"P[{p.Id}]_{id}";

    public Polyomino P => p;
}