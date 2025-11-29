using Csp.Helpers;
using Csp.Interfaces;

namespace Csp.Impl;

public static class Gac
{
    // Arc consistency runner.
    //    ...GAC is pretty goated, if a bit computationally expensive. With the sauce, even.

    public static void Run<T>(ICollection<IConstraint<T>> constraints, IDictionary<IVariable, IDomain<T>> domains)
    {
        var queue = new Queue<(IConstraint<T>, IVariable)>();

        // initialize queue with all relevant (C, V) tuples
        foreach (var c in constraints)
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
            if (domains[variable].Values.Count == 0)
            {
                throw new Exception($"Contradiction found - {variable.Name} no longer has a valid solution");
            }
            
            // enqueue neighbors - (Ci, N) where Ci contains variable in its scope.
            foreach (var neighborConstraint in constraints.Where(c => c.Scope.Contains(variable)))
            {
                foreach (var neighborVariable in neighborConstraint.Scope)
                {
                    queue.Enqueue((neighborConstraint, neighborVariable));
                }
            }
        }
    }

    private static bool Revise<T>(IConstraint<T> constraint, IVariable variable, IDictionary<IVariable, IDomain<T>> domains)
    {
        // return value - if we change v's domain, we let the runner know so it rechecks affected constraints.
        var somethingChanged = false;

        // we want all remaining possible combos of solutions for relevant variables.
        var otherVariables = constraint.Scope.Where(v => v != variable).ToList();
        var otherVariableTuples = otherVariables.Select(v => domains[v].Values).CartesianProduct().ToList();

        foreach (var xValue in domains[variable].Values.ToList()) // copy because we may mutate
        {
            // is there a possible scenario right now where xValue is valid?
            var hasSupport = false;
            
            foreach (var possibleState in otherVariableTuples)
            {
                var assignment = new Assignment<T>(variable, xValue, otherVariables, possibleState.ToList());
                if (constraint.IsSatisfied(assignment))
                {
                    hasSupport = true;
                    break;
                }
            }
            
            if (!hasSupport)
            {
                Console.WriteLine(
                    $"[debug] Checking v=[{variable.Name}] for c=[{constraint.Name}], desc=[{constraint.Description}] -> determined value {xValue} has no support");
                domains[variable].Values.Remove(xValue);
                somethingChanged = true;
            }
        }
        
        return somethingChanged;
    }
}