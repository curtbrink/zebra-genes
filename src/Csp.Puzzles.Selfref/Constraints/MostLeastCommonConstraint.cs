using Csp.Core.Models.Models.Domain.Interfaces;
using Csp.Core.Models.Models.Variable.Interfaces;
using Csp.Helpers;

namespace Csp.Puzzles.Selfref.Constraints;

public class MostLeastCommonConstraint(
    IOrderedVariable owner,
    IReadOnlyCollection<IOrderedVariable> scope,
    IReadOnlyCollection<string?> choiceList,
    bool isLeastCommon = false,
    bool isMostCommonCount = false) : BaseSelfRefConstraint<string?>(scope, choiceList)
{
    public override string Name => $"{_descriptor}Common{(isMostCommonCount ? "Count" : "")}";
    public override string Description => $"{_descriptor} common answer is {owner.Name}={{{OptionString}}}";

    private readonly string _descriptor = isLeastCommon ? "Least" : "Most";
    
    public override bool IsSatisfiable(IDomainStore<string> domains)
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

        foreach (var question in QuestionScope)
        {
            var qDomain = domains.GetDomain(question).Values.ToList();
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

        if (isMostCommonCount)
        {
            // map to ints and check t value instead -
            return ownerOptions.Select(GetChoice)
                .Any(choice => CanMostCommonCountBe(minimums, maximums, int.Parse(choice!)));
        }
        
        return ownerOptions.Select(GetChoice).Any(choice => IsValid(minimums, maximums, choice));
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

    private bool CanMostCommonCountBe(IDictionary<string, int> minimums, IDictionary<string, int> maximums, int tValue)
        => ExistsViaIntegerFeasibility(minimums, maximums, false, null, tValue);

    private bool ExistsViaIntegerFeasibility(IDictionary<string, int> minimums, IDictionary<string, int> maximums,
        bool checkLeastCommon = false, string? checkForWinner = null, int? checkForT = null)
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
            // if we're checking for a _count_ we want to include a sole winner.
            var minSubsetSize = checkForT != null ? 1 : 2;
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
            // if we're checking for a specific T, and it's not in this range, also fail.
            if (tmin > tmax || tmin > checkForT || tmax < checkForT)
            {
                continue;
            }
            
            // see if we can feasibly force a winner or tie with this S and range of T
            var tRangeMin = checkForT ?? tmin;
            var tRangeMax = checkForT ?? tmax;
            for (var t = tRangeMin; t <= tRangeMax; t++)
            {
                // we know each option in S has count T exactly
                var assignedCount = optionSubset.Count * t;
                
                // short circuit if we know this won't work
                if (assignedCount > QuestionScope.Count)
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
                    if (checkLeastCommon)
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
                if (isValid && lowerBoundTotal <= QuestionScope.Count && QuestionScope.Count <= upperBoundTotal)
                {
                    return true;
                }
            }
        }
        
        // exhausted all options.
        return false;
    }
}