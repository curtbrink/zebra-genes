using Csp.Helpers;
using Csp.Interfaces;

namespace Csp.Impl;

public class ContradictionException(string message) : Exception(message);

public static class Gac
{
    // Arc consistency runner.
    //    ...GAC is pretty goated, if a bit computationally expensive. With the sauce, even.

    public static IDictionary<IVariable, IDomain<T>> RunWithBacktrackingSearch<T>(ICollection<IConstraint<T>> constraints,
        IDictionary<IVariable, IDomain<T>> domains)
    {
        // recursively 
        // base case: solved puzzle
        if (domains.Values.All(v => v.Values.Count == 1))
        {
            return domains;
        }
        
        // else pick the "closest to solve" => question with fewest remaining choices (MRV)
        // then try each value, run a GAC, and recurse.
        // if we never find a contradiction we know we have a good solution that we bubble up to the top.
        var minDomainSize = domains.Values.Min(v => v.Values.Count == 1 ? int.MaxValue : v.Values.Count);
        var firstVariableWithMin = domains.Keys.First(k => domains[k].Values.Count == minDomainSize);

        var firstVariableDomain = domains[firstVariableWithMin];

        foreach (var vValue in firstVariableDomain.Values.ToList())
        {
            Console.WriteLine($"[SEARCH] Looking for possible solutions with {firstVariableWithMin.Name}={vValue} ...");
            var domainCopy = DeepCopyDomains(domains);
            
            // hard set that question's choice to vValue
            domainCopy[firstVariableWithMin] = new Domain<T>([vValue]);

            try
            {
                // GAC to stable. this will throw a ContradictionException if our hypothetical choice
                // directly propagates to a contradiction.
                Run(constraints, domainCopy);

                // we now either have a complete solution or a partial solution without contradictions.
                // recurse to get to a complete solution (the only base case).
                // this call will throw a ContradictionException if our search doesn't find any valid solutions
                // that include our partial solution.
                var result = RunWithBacktrackingSearch<T>(constraints, domainCopy);

                // ladies and gentlemen...we got 'em
                // if we're still here, congrats! it's a full solution.
                return result;
            }
            catch (ContradictionException ce)
            {
                // our hypothetical partial solution didn't yield any full solutions.
                Console.WriteLine(ce.Message);
                continue;
            }
        }
        
        // no option for this variable yielded a good solution.
        Console.WriteLine($"[SEARCH] No valid solutions found for {firstVariableWithMin.Name} here!");
        throw new ContradictionException($"Could not find a valid solution for {firstVariableWithMin.Name}");
    }

    private static IDictionary<IVariable, IDomain<T>> DeepCopyDomains<T>(IDictionary<IVariable, IDomain<T>> domains)
    {
        var newDict = new Dictionary<IVariable, IDomain<T>>();
        foreach (var v in domains.Keys)
        {
            newDict[v] = new Domain<T>(domains[v].Values.ToList());
        }

        return newDict;
    }

    public static IDictionary<IVariable, IDomain<T>> Run<T>(ICollection<IConstraint<T>> constraints, IDictionary<IVariable, IDomain<T>> domains)
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
                throw new ContradictionException($"Contradiction found - {variable.Name} no longer has a valid solution");
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

        return domains;
    }

    private static bool Revise<T>(IConstraint<T> constraint, IVariable variable, IDictionary<IVariable, IDomain<T>> domains)
    {
        // return value - if we change v's domain, we let the runner know so it rechecks affected constraints.
        var somethingChanged = false;

        List<T> xValuesToTry = [.. domains[variable].Values];
        foreach (var xValue in xValuesToTry) // copy because we may mutate
        {
            // is there a possible scenario right now where xValue is valid?
            if (!constraint.IsSatisfiable(variable, xValue, domains))
            {
                Console.WriteLine(
                    $"[debug] Checking v=[{variable.Name}] for c=[{constraint.Name}], desc=[{constraint.Description}]");
                Console.WriteLine($" -> determined value {xValue} has no support");
                domains[variable].Values.Remove(xValue);
                
                // pretty print domains at this point
                Console.WriteLine($"[debug] new domains => {PrettyPrintDomains(domains)}");
                somethingChanged = true;
            }
        }
        
        return somethingChanged;
    }

    private static string PrettyPrintDomains<T>(IDictionary<IVariable, IDomain<T>> domains)
    {
        var domainStrings = domains.Keys.Select(k => (k, domains[k].Values.ToList()))
            .ToDictionary(p => p.k, p => p.Item2);
        var domainPrettied =
            string.Join(" ",
                domainStrings.Keys.Select(k => $"{k.Name}={{{string.Join("", domainStrings[k])}}}"));
        return domainPrettied;
    }
}