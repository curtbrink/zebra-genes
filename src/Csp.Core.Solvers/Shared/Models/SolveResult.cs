using Csp.Core.Models.Models.Variable.Interfaces;

namespace Csp.Core.Solvers.Shared.Models;

public record SolveResult<T>(SolveStatus Status, string Message, IDictionary<IVariable, T>? solution);