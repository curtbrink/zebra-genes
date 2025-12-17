using Csp.Core.Models.Models.Csp.Interfaces;
using Csp.Core.Solvers.Shared.Models;

namespace Csp.Core.Solvers.Shared.Interfaces;

public interface ISolver<T>
{
    public SolveResult<T> Solve(ICsp<T> csp);
}