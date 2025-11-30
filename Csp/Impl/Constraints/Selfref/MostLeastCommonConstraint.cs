using Csp.Helpers;
using Csp.Interfaces;

namespace Csp.Impl.Constraints.Selfref;

public class MostLeastCommonConstraint(
    IVariable owner,
    IEnumerable<IVariable> scope,
    IEnumerable<string?> choiceList,
    bool isLeastCommon = false) : BaseSelfRefConstraint<string?>(scope, choiceList)
{
    public override string Name => $"{_descriptor}Common";
    public override string Description => $"{_descriptor} common answer is {owner.Name}={{{OptionString}}}";

    private readonly string _descriptor = isLeastCommon ? "Least" : "Most";
    
    public override bool IsSatisfiable(IVariable v, string val, IDictionary<IVariable, IDomain<string>> domains)
    {
        // first step: count all actuals and hypotheticals
        // initialize dicts
        // actualCounts is number of concrete N
        var minimums = new Dictionary<string, int>();
        var maximums = new Dictionary<string, int>();
        foreach (var o in Options)
        {
            minimums[o] = 0;
            maximums[o] = 0;
        }

        List<string> ownerOptions = [];

        foreach (var question in Scope)
        {
            var qDomain = question == v ? [val] : domains[question].Values.ToList();
            if (question == owner)
            {
                // we'll have to check for these
                ownerOptions = qDomain.ToList();
            }
            
            foreach (var o in qDomain)
            {
                maximums[o] += 1;
                if (qDomain.Count == 1)
                {
                    minimums[o] += 1;
                }
            }
        }
        
        // now we have minimum and maximum counts for each option.
        // check if any support this v assignment.
        return ownerOptions.Select(oo => ChoiceList[oo]).Any(choice => IsValid(minimums, maximums, choice));
    }

    private bool IsValid(IDictionary<string, int> minimums, IDictionary<string, int> maximums, string? choice)
    {
        // shepherd choice based on constraint type
        if (choice == null)
        {
            return isLeastCommon
                ? CanLeastCommonTieExist(minimums, maximums)
                : CanMostCommonTieExist(minimums, maximums);
        }

        return isLeastCommon
            ? CanOptionBeLeastCommon(minimums, maximums, choice)
            : CanOptionBeMostCommon(minimums, maximums, choice);
    }

    private bool CanOptionBeLeastCommon(IDictionary<string, int> minimums, IDictionary<string, int> maximums,
        string option) => ExistsViaIntegerFeasibility(minimums, maximums, true, option);

    private bool CanLeastCommonTieExist(IDictionary<string, int> minimums, IDictionary<string, int> maximums) =>
        ExistsViaIntegerFeasibility(minimums, maximums, true);

    private bool CanOptionBeMostCommon(IDictionary<string, int> minimums, IDictionary<string, int> maximums,
        string option) => ExistsViaIntegerFeasibility(minimums, maximums, false, option);
    
    private bool CanMostCommonTieExist(IDictionary<string, int> minimums, IDictionary<string, int> maximums) =>
        ExistsViaIntegerFeasibility(minimums, maximums, false);

    private bool ExistsViaIntegerFeasibility(IDictionary<string, int> minimums, IDictionary<string, int> maximums,
        bool isLeastCommon = false, string? checkForWinner = null)
    {
        // generic function to see if there is a possible clear winner or a feasible tie via integer feasibility.
        // tries to find subsets of options where a valid solution exists.
        
        var allOptions = Options.ToList();
        var optionSubsets = new List<List<string>>();
        if (checkForWinner != null)
        {
            // we can reuse this logic to check if there's a feasible SOLE WINNER for most/least common.
            // otherwise we default to checking for a valid tie.
            optionSubsets = [[checkForWinner]];
        }
        else
        {
            var minSubsetSize = 2;
            var maxSubsetSize = Options.Count;
            for (var i = minSubsetSize; i <= maxSubsetSize; i++)
            {
                optionSubsets.AddRange(allOptions.GetAllSubsetsOfSize(i));
            }
        }
        
        // check each subset for a viable solution
        foreach (var optionSubset in optionSubsets)
        {
            // narrow our T value down for this subset
            var tmin = int.MinValue;
            var tmax = int.MaxValue;
            foreach (var o in optionSubset)
            {
                tmin = Math.Max(tmin, minimums[o]);
                tmax = Math.Min(tmax, maximums[o]);
            }
            // if there is no overlap (i.e. count(s1) can't ever equal count(s2)) fail
            if (tmin > tmax)
            {
                continue;
            }
            
            // see if we can feasibly force a winner or tie with this S and range of T
            for (var t = tmin; t <= tmax; t++)
            {
                // we know each option in S has count T exactly
                var assignedCount = optionSubset.Count * t;
                
                // short circuit if we know this won't work
                if (assignedCount > Scope.Count)
                {
                    continue;
                }
                
                // otherwise see if we can make this combo work
                var lowerBoundTotal = assignedCount;
                var upperBoundTotal = assignedCount;
                
                // check the other options' min and max
                var isValid = true;
                foreach (var o2 in allOptions.Where(o => !optionSubset.Contains(o)))
                {
                    // this is where the least/most common distinction lies
                    int optionLowerBound, optionUpperBound;
                    // most common tie:
                    if (isLeastCommon)
                    {
                        // clamp option not in S floor to T+1
                        optionLowerBound = Math.Max(minimums[o2], t + 1);
                        optionUpperBound = maximums[o2];
                    }
                    else
                    {
                        // clamp option not in S ceiling to T-1
                        optionLowerBound = minimums[o2];
                        optionUpperBound = Math.Min(maximums[o2], t - 1);
                    }

                    if (optionLowerBound > optionUpperBound)
                    {
                        isValid = false;
                        break;
                    }
                    
                    lowerBoundTotal += optionLowerBound;
                    upperBoundTotal += optionUpperBound;
                }
                // now see if our totals can produce a valid solution
                if (isValid && lowerBoundTotal <= Scope.Count && Scope.Count <= upperBoundTotal)
                {
                    return true;
                }
            }
        }
        
        // exhausted all options.
        return false;
    }
    
    
    // PROBLEM REFORMULATED...
    // we can leave "can be most/least common" alone I think.
    // we specifically care about ties. however, seems like the math would work for a subset size 1 too. let's see.
    
    /*
     * definitions
     * M = number of options = 5
     * N = total number of questions
     *
     * for any option in M, call it J -
     * minimums[j] = number of questions assigned to J concretely
     * maximums[j] = maximum possible J assignments given domains.
     * possibleAdd[j] = maximums[j] - minimums[j]
     *
     * Q: does there exist an assignment of remaining questions to options (respecting domains) such that:
     * - for a most-common tie:
     *   - there is a subset S of options and integer T such that
     *     - every option in S has count(option) = T
     *     - every option not in S has count(option) < T
     *     - every option has minimums[option] <= count(option) <= maximums[option]
     *     - sum(count(option)) = N
     * - for a least-common tie:
     *   - there is a subset S of options and integer T such that
     *     - every option in S has count(option) = T
     *     - every option not in S has count(option) > T
     *     - every option has minimums[option] <= count(option) <= maximums[option]
     *     - sum(count(option)) = N
     *
     * integer feasibility for subset S and integer T for most-common:
     * for each option j => if j is in S, and if T < minimums[j] or T > maximums[j] => fail because T = count(j)
     *                   => if j is not in S, count(j) < T => fail if minimums[j] > min(T, maximums[j]).
     *
     * what T do we check for a given S??
     * - Tfloor = max(min(S1), min(S2), ...);
     * - Tceil = min(max(S1), max(S2), ...);
     * - Tfloor - Tceil is the overlap in possible totals for S.
     * - -> if empty (Tfloor > Tceil), not feasible.
     * - -> now we need to compare against N.
     * - -> let count(S(-1)) be the sum of the minimum (already-assigned) counts for options not in S.
     * - -> so, if size(S) * T + count(S(-1)) > N, not feasible.
     *
     * ex: most common tie
     * AB both min3 max4.
     * N = 8, already 3 A 3 B, 1 unassigned with domain {A,B} so let's say we're at 3 A 3 B 1 C.
     * C would be min1 max1, D would be min0 max0, E would be min0 max0.
     * checking (AB, 3..4) for S and T:
     * ok = true
     * loop 1: check T=3
     * lb = 0
     * ub = 0
     * loop options
     * S contains A so lb = 3 ub = 3
     * S contains B so lb = 6 ub = 6
     * S does not contain C. cLb = min(C) = 1, cUb = min(max(C), T-1) = min(1, 2) = 1. lb = 7 ub = 7.
     * S does not contain D. dLb = min(D) = 0, dUb = min(max(D), T-1) = min(0, 2) = 0. lb = 7 ub = 7.
     * S does not contain E. eLb = min(E) = 0, eUb = min(max(E), T-1) = min(0, 2) = 0. lb = 7 ub = 7.
     * bounds are ok => ub is >= lb
     * lb <= N=8 <= ub? no, ub is 7.
     * loop 2: check T=4
     * lb = 0
     * ub = 0
     * loop options
     * S contains A so lb = 4 ub = 4
     * S contains B so lb = 8 ub = 8
     * S does not contain C. cLb = min(C) = 1, cUb = min(max(C), T-1) = min(1, 2) = 1. lb = 9 ub = 9.
     * ...same for D and E
     * bounds are ok => ub is >= lb
     * lb <= N=8 <= ub? no, lb is 9. => tie not possible for these S and T values.
     *
     * how about another example where tie is possible?
     * let's say: 10 Qs
     * A min 2 max 4
     * B min 2 max 4
     * C min 2 max 2
     * D min 2 max 4
     * E min 0 max 0
     * -> clearly 2 unassigned with domain ABD each
     * -> clearly tie would be possible at T=3
     *
     * can S AD T 4 be tie?
     * A 4 B 2 C 2 D 4 would be 12 so no
     * can S AD T 3 be tie?
     * lb = 0 ub = 0
     * S contains A so lb 3 ub 3
     * S does not contain B. bLb = min(b) = 2, bUb = min(max(b),T-1) = min(4, 2) = 2 so lb 5 ub 5
     * S does not contain C. cLb = min(c) = 2, cUb = min(max(c),T-1) = min(2, 2) = 2 so lb 7 ub 7
     * S contains D so lb 10 ub 10
     * S does not contain E. eLb = min(e) = 2, eUb = min(max(e),T-1) = min(0, 2) = 2 so lb 10 ub 10
     * lb <= N=10 <= ub ? yes, lb = 10 = ub. tie is possible.
     * 
     */
}