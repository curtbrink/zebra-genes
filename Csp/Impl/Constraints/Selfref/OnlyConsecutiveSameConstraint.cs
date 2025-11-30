using Csp.Interfaces;

namespace Csp.Impl.Constraints.Selfref;

public class OnlyConsecutiveSameConstraint(
    IOrderedVariable owner,
    IEnumerable<IOrderedVariable> scope,
    IEnumerable<List<int>> choiceList)
    : BaseSelfRefConstraint<List<int>>(scope, choiceList)
{
    public override string Name => "OnlyConsecutiveSame";
    public override string Description => $"Only {_consecutiveN} consecutive same starts with {owner.Name}={{{OptionString}}}";
    
    private new string OptionString => string.Join("|", OptionListItems);
    private List<string> OptionListItems => ChoiceList.Keys.Select(k => $"{k}={ChoiceList[k].First()}").ToList();
    
    private readonly int _consecutiveN = choiceList.ElementAt(0).Count;

    public override bool IsSatisfiable(IVariable v, string val, IDictionary<IVariable, IDomain<string>> domains)
    {
        // two ways to falsify:
        // 1. a consecutive slice exists that isn't in the available slices.
        // 2. none of the available slices have a non-empty intersection.
        var possibleSlices = v == owner ? [ChoiceList[val]] : domains[owner].Values.Select(o => ChoiceList[o]).ToList();
        var possibleSliceStarters = possibleSlices.Select(s => s[0]).ToList();
        
        // 1. check for existing slices
        // note: number of slices of size N in a set of size S:
        // S = 8 -> N = 3 -> 0..2 - 5..7 -> 6
        // ending index = Q - N
        for (var i = 0; i <= Scope.Count - _consecutiveN; i++)
        {
            var qList = Scope.ToList().Slice(i, _consecutiveN);
            var qDomains = qList.Select(q => q == v ? [val] : domains[q].Values.ToList()).ToList();
            if (qDomains.Any(d => d.Count > 1))
            {
                continue; // some unassigned, not a consec same slice
            }

            var expected = qDomains[0][0];
            if (qDomains.Any(d => d[0] != expected))
            {
                continue; // not consec same slice
            }
            
            // all are assigned, all are the same
            // if it isn't in our list though we dun goofed
            var startingQIndex = i + 1;
            return possibleSliceStarters.Contains(startingQIndex);
        }
        
        // 2. check that there's a possible slice with a valid intersection of options in domain
        foreach (var i in possibleSliceStarters)
        {
            var qSlice = Scope.ToList().Slice(i - 1, _consecutiveN);
            var qDomains = qSlice.Select(q => q == v ? [val] : domains[q].Values.ToList()).ToList();
            // get intersection of domains
            var intersection = qDomains[0];
            for (var j = 1; j < qDomains.Count; j++)
            {
                intersection = intersection.Where(o => !qDomains[j].Contains(o)).ToList();
            }

            if (intersection.Count > 0)
            {
                // supported
                return true;
            }
        }

        return false;
    }
}