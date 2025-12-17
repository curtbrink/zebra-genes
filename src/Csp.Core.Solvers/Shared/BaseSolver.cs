using Csp.Core.Solvers.Shared.Helpers;
using Csp.Core.Solvers.Shared.Models;
using Csp.Core.Solvers.Shared.Models.Interfaces;

namespace Csp.Core.Solvers.Shared;

public abstract class BaseSolver<T>
{
    protected static SolveResult<T> GenerateResult(IMutableDomainStore<T> domainStore)
    {
        var solvedDomains = domainStore.GetAllDomains();
        if (solvedDomains.Any(kvp => kvp.Value.Values.Count == 0))
        {
            return new SolveResult<T>(SolveStatus.NotSatisfied,
                "At least one variable had an empty domain after solve/propagation", null);
        }

        if (solvedDomains.Any(kvp => kvp.Value.Values.Count > 1))
        {
            return new SolveResult<T>(SolveStatus.Incomplete,
                "No contradictions found, but did not find a full solution", null);
        }

        // else we have a full solution!
        return new SolveResult<T>(SolveStatus.Satisfied, "Full solution found", domainStore.ToSolvedDomains());
    }
}