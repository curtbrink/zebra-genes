using Csp.Objects.Variables.Interfaces;
using Csp.Types.Polyomino;

namespace Csp.Objects.Variables.Impl;

public class PolyominoVariable(string id, Polyomino p) : IVariable
{
    public string Name => $"P[{p.Id}]_{id}";

    public Polyomino P => p;
}