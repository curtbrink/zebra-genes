using Csp.Core.Models.Models.Variable.Interfaces;

namespace Csp.Core.Models.Models.Variable;

// todo this should live in the Polyomino-specific puzzle project

// public class PolyominoVariable(string id, Polyomino p) : IVariable
public class PolyominoVariable(string id, string p) : IVariable
{
    // public string Name => $"P[{p.Id}]_{id}";
    public string Name => $"P[{p}]_{id}";

    // public Polyomino P => p;
}