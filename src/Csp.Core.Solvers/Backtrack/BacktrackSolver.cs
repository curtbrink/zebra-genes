using Csp.Core.Models.Models.Csp.Interfaces;
using Csp.Core.Models.Models.Variable.Interfaces;
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
    private readonly List<IPruner<ISearchState<T>, T>> _pruners = [];

    private ISearchState<T>? _searchState;

    private int _layerThreshold = 0;
    
    public void AddPruner(IPruner<ISearchState<T>, T> pruner)
    {
        logger.LogDebug("Adding pruner {Pruner}", typeof(T).Name);
        _pruners.Add(pruner);
    }

    public SolveResult<T> Solve(ICsp<T> csp, ISearchState<T>? searchState)
    {
        var domainStore = new MutableDomainStore<T>(csp.Domains);
        
        _searchState = searchState;
        _searchState?.DomainStore = domainStore;
        foreach (var pruner in _pruners)
        {
            pruner.SetSearchState(searchState);
        }

        var thres = 0.4d;
        _layerThreshold = Convert.ToInt32(thres * csp.Variables.Count);
        
        try
        {
            return Recurse(csp, domainStore, 0);
        }
        catch (ContradictionException ce)
        {
            logger.LogDebug("BacktrackSolver threw a ContradictionException: {CeMessage}", ce.Message);
            return new SolveResult<T>(SolveStatus.NotSatisfied, ce.Message, null);
        }
    }
    
    public SolveResult<T> Recurse(ICsp<T> csp, IMutableDomainStore<T> domainStore, int searchLayer)
    {
        // recursively 
        // base case: full solution
        if (domainStore.IsSolved())
        {
            return new SolveResult<T>(SolveStatus.Satisfied, "Full solution found", domainStore.ToSolvedDomains());
        }

        var skipPrune = searchLayer < _layerThreshold || _searchState is null;
        
        // else pick the "closest to solve" => question with fewest remaining choices (MRV)
        // then try each value, run a GAC, and recurse.
        // if we never find a contradiction we know we have a good solution that we bubble up to the top.
        var domains = domainStore.GetAllDomains();
        IVariable? minDomainKey = null;
        IReadOnlySet<T>? minDomain = null;
        var minDomainSize = int.MaxValue;
        foreach (var k in domains.Keys)
        {
            var d = domains[k].Values;
            var c = d.Count;
            if (c > 1 && c < minDomainSize)
            {
                minDomainKey = k;
                minDomain = d;
                minDomainSize = c;
            }
        }

        if (minDomainKey is null || minDomain is null)
            throw new ContradictionException("not solved but also somehow no domains to backtrack.");
        
        List<T> candidates = [..minDomain];
        foreach (var vValue in candidates)
        {
            logger.LogDebug("[SEARCH] Looking for possible solutions with {VarName}={TestValue} ...",
                minDomainKey.Name, vValue);
            
            var snapshot = domainStore.Clone();
            
            // hard set that variable's domain to candidate
            snapshot.GetMutableDomain(minDomainKey).SetTo(vValue);
            _searchState?.DomainStore = snapshot;

            try
            {
                // Run our inference solver to stable. this will throw a ContradictionException if our assignment
                // directly propagates to a contradiction.
                inferenceSolver.Propagate(csp, snapshot);
                
                // NEW: see if we're SOL and yeet if one of our pruners raises the white flag
                if (!skipPrune)
                {
                    foreach (var pruner in _pruners)
                    {
                        if (pruner.ShouldPrune())
                            throw new ContradictionException(
                                $"Pruner {pruner.GetType().Name} decided this branch should be pruned");
                    }
                }

                // we now either have a complete solution or a partial solution without contradictions.
                // recurse to get to a complete solution (the only base case).
                // this call will throw a ContradictionException if our search doesn't find any valid solutions
                // that include our partial solution.
                var result = Recurse(csp, snapshot, searchLayer + 1);

                // ladies and gentlemen...we got 'em
                // if we're still here, congrats! it's a full solution.
                return result;
            }
            catch (ContradictionException ce)
            {
                // our hypothetical partial solution didn't yield any full solutions.
                logger.LogDebug(ce.Message);
            }
        }
        
        // no option for this variable yielded a good solution.
        logger.LogDebug("[SEARCH] No valid solutions found for {VarName} here!", minDomainKey.Name);
        throw new ContradictionException($"Could not find a valid solution for {minDomainKey.Name}");
    }
}