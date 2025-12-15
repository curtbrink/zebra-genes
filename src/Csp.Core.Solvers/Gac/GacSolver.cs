using Csp.Core.Models.Models.Constraint.Interfaces;
using Csp.Core.Models.Models.Csp.Interfaces;
using Csp.Core.Models.Models.Variable.Interfaces;
using Csp.Core.Solvers.Shared;
using Csp.Core.Solvers.Shared.Exceptions;
using Csp.Core.Solvers.Shared.Helpers;
using Csp.Core.Solvers.Shared.Interfaces;
using Csp.Core.Solvers.Shared.Models;
using Csp.Core.Solvers.Shared.Models.Interfaces;
using Microsoft.Extensions.Logging;

namespace Csp.Core.Solvers.Gac;

public class GacSolver<T>(ILogger<GacSolver<T>> logger) : BaseSolver<T>, IInferenceSolver<T>
{
    // Arc consistency runner.
    //    ...GAC is pretty goated, if a bit computationally expensive. With the sauce, even.
    
    public SolveResult<T> Solve(ICsp<T> csp)
    {
        var domainStore = new MutableDomainStore<T>(csp.Domains);

        try
        {
            Propagate(csp, domainStore);
        }
        catch (ContradictionException ce)
        {
            logger.LogDebug("GacSolver threw a ContradictionException: {CeMessage}", ce.Message);
            return new SolveResult<T>(SolveStatus.NotSatisfied, ce.Message, null);
        }
        
        // check solve status
        return GenerateResult(domainStore);
    }

    public void Propagate(ICsp<T> csp, IMutableDomainStore<T> domains)
    {
        var queue = new Queue<(IConstraint<T>, IVariable)>();

        // initialize queue with all relevant (C, V) tuples
        foreach (var c in csp.Constraints)
        {
            foreach (var v in c.Scope)
            {
                queue.Enqueue((c, v));
            }
        }

        // run until nothing changes
        while (queue.Count > 0)
        {
            var (constraint, variable) = queue.Dequeue();

            if (!Revise(constraint, variable, domains))
            {
                // nothing changed
                continue;
            }
            
            // otherwise something changed and we have to recheck affected constraints.
            // quick contradiction check - variable still needs a possible solution
            if (domains.GetMutableDomain(variable).Values.Count == 0)
            {
                throw new ContradictionException($"Contradiction found - {variable.Name} no longer has a valid solution");
            }
            
            // enqueue neighbors - (Ci, N) where Ci contains variable in its scope.
            foreach (var neighborConstraint in csp.Constraints.Where(c => c.Scope.Contains(variable)))
            {
                foreach (var neighborVariable in neighborConstraint.Scope)
                {
                    queue.Enqueue((neighborConstraint, neighborVariable));
                }
            }
        }
    }
    
    private bool Revise(IConstraint<T> constraint, IVariable variable, IMutableDomainStore<T> domains)
    {
        // return value - if we change v's domain, we bubble it up so it rechecks affected constraints.
        var somethingChanged = false;

        List<T> candidates = [..domains.GetMutableDomain(variable).Values];
        List<T> candidatesToCheck = [..candidates];
        foreach (var xValue in candidatesToCheck) // copy because we may mutate
        {
            // is there a possible scenario right now where xValue is valid?
            domains.GetMutableDomain(variable).SetTo(xValue);
            if (!constraint.IsSatisfiable(domains))
            {
                logger.LogDebug("Checking v=[{VarName}] for c=[{ConstraintName}], desc=[{ConstraintDesc}]",
                    variable.Name, constraint.Name, constraint.Description);
                logger.LogDebug(" -> determined value {XValue} has no support", xValue);
                
                candidates.Remove(xValue);
                
                // pretty print domains at this point
                somethingChanged = true;
            }
        }

        // after pruning this variable's domains, make sure to set it back to our valid candidates.
        domains.GetMutableDomain(variable).SetTo(candidates);
        
        logger.LogDebug("[debug] new domains => {PrettyDomains}", domains.PrettyPrint());
        
        return somethingChanged;
    }
}