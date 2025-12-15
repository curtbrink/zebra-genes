using Csp.Core.Models.Models.Csp.Interfaces;
using Csp.Core.Solvers.Shared;
using Csp.Core.Solvers.Shared.Exceptions;
using Csp.Core.Solvers.Shared.Helpers;
using Csp.Core.Solvers.Shared.Interfaces;
using Csp.Core.Solvers.Shared.Models;
using Csp.Core.Solvers.Shared.Models.Interfaces;
using Microsoft.Extensions.Logging;

namespace Csp.Core.Solvers.Backtrack;

public class BacktrackSolver<T>(ILogger<BacktrackSolver<T>> logger, IInferenceSolver<T> inferenceSolver)
    : BaseSolver<T>, ISearchSolver<T>
{
    public SolveResult<T> Solve(ICsp<T> csp)
    {
        var domainStore = new MutableDomainStore<T>(csp.Domains);

        try
        {
            Recurse(csp, domainStore);
        }
        catch (ContradictionException ce)
        {
            logger.LogDebug("BacktrackSolver threw a ContradictionException: {CeMessage}", ce.Message);
            return new SolveResult<T>(SolveStatus.NotSatisfied, ce.Message, null);
        }
        
        // check solve status
        return GenerateResult(domainStore);
    }
    
    public SolveResult<T> Recurse(ICsp<T> csp, IMutableDomainStore<T> domainStore)
    {
        // recursively 
        // base case: full solution
        if (domainStore.IsSolved())
        {
            return new SolveResult<T>(SolveStatus.Satisfied, "Full solution found", domainStore.ToSolvedDomains());
        }
        
        // else pick the "closest to solve" => question with fewest remaining choices (MRV)
        // then try each value, run a GAC, and recurse.
        // if we never find a contradiction we know we have a good solution that we bubble up to the top.
        var domains = domainStore.GetAllDomains();
        var minDomainSize = domains.Values
            .Min(v => v.Values.Count == 1 ? int.MaxValue : v.Values.Count);
        var firstVariableWithMin = domains.Keys.First(k => domains[k].Values.Count == minDomainSize);

        var firstVariableDomain = domains[firstVariableWithMin];

        List<T> candidates = [..firstVariableDomain.Values];
        foreach (var vValue in candidates)
        {
            logger.LogDebug("[SEARCH] Looking for possible solutions with {VarName}={TestValue} ...",
                firstVariableWithMin.Name, vValue);
            
            var snapshot = domainStore.Clone();
            
            // hard set that variable's domain to candidate
            snapshot.GetMutableDomain(firstVariableWithMin).SetTo(vValue);

            try
            {
                // Run our inference solver to stable. this will throw a ContradictionException if our assignment
                // directly propagates to a contradiction.
                inferenceSolver.Propagate(csp, snapshot);

                // we now either have a complete solution or a partial solution without contradictions.
                // recurse to get to a complete solution (the only base case).
                // this call will throw a ContradictionException if our search doesn't find any valid solutions
                // that include our partial solution.
                var result = Recurse(csp, snapshot);

                // ladies and gentlemen...we got 'em
                // if we're still here, congrats! it's a full solution.
                return result;
            }
            catch (ContradictionException ce)
            {
                // our hypothetical partial solution didn't yield any full solutions.
                Console.WriteLine(ce.Message);
            }
        }
        
        // no option for this variable yielded a good solution.
        logger.LogDebug("[SEARCH] No valid solutions found for {VarName} here!", firstVariableWithMin.Name);
        throw new ContradictionException($"Could not find a valid solution for {firstVariableWithMin.Name}");
    }
}