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

    protected override bool IsSatisfiableInternal(IDictionary<IOrderedVariable, IDomain<string>> domains)
    {
        // is there at least one supported answer in owner's domain?
        // supported if:
        //  - the Q's in that answer have a potential shared answer
        //  - no other consecutive set of N is _forced_

        var possibleAnswers = domains[owner].Values.ToList();
        foreach (var candidate in possibleAnswers)
        {
            // can this set of questions all be equal?
            var candidateSlice = ChoiceList[candidate];
            var firstId = candidateSlice.Min();
            var sliceDomains = candidateSlice.Select(c => scope.First(va => va.Id == c))
                .Select(ov => domains[ov].Values.ToList()).ToList();
            var intersection = sliceDomains[0];
            for (var i = 1; i < sliceDomains.Count; i++)
            {
                intersection = intersection.Intersect(sliceDomains[i]).ToList();
            }

            if (intersection.Count == 0) continue;
            
            // is there a forced pair anywhere else?
            var isValid = true;
            for (var i = 1; i <= QuestionScope.Count - (_consecutiveN - 1); i++)
            {
                if (i == firstId) continue; // this is our valid one!

                var list = new List<int>();
                for (var j = 0; j < _consecutiveN; j++)
                {
                    list.Add(i + j);
                }

                var otherSliceDomains = list.Select(id => scope.First(va => va.Id == id))
                    .Select(ov => domains[ov].Values.ToList()).ToList();

                if (otherSliceDomains.Any(d => d.Count > 1)) continue;
                
                var firstAnswer = otherSliceDomains[0][0];

                if (otherSliceDomains.Any(d => !d.Contains(firstAnswer))) continue;
                
                isValid = false;
                break;
            }

            if (isValid) return true;
        }

        return false;
    }
}