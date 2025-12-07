using Csp.Objects.Domain;
using Csp.Objects.Variables.Interfaces;

namespace Csp.Impl.Constraints.Selfref;

// this type can cover all of these:
// first X
// last X -> same as first X if the questions were reversed
// closest X after #Y -> scope is a narrower subset of Qs
// closest X before #Y -> same as closest after, with same narrower scope and questions reversed

public class FirstWithChoiceConstraint(
    IOrderedVariable owner,
    IEnumerable<IOrderedVariable> rangeToCheck,
    IEnumerable<int?> choiceList,
    string? expected = null,
    bool isReverse = false,
    bool isDeterminedByOwner = false) : BaseSelfRefConstraint<int?>(rangeToCheck.Contains(owner) ? rangeToCheck : [..rangeToCheck, owner], choiceList)
{
    // can also do:
    // - last with choice
    // - first/last with same as owner
    
    public override string Name => "FirstWithChoice";
    public override string Description => $"{_descriptor} Q with {_whichChoice} is {owner.Name}=>{{{OptionString}}}";

    private readonly string _descriptor = isReverse ? "Last" : "First";
    private readonly string _whichChoice = isDeterminedByOwner ? $"same choice as {owner.Name}" : $"choice {expected}";

    private readonly Comparison<IOrderedVariable> _comparer = isReverse ? (a, b) => b.Id - a.Id : (a, b) => a.Id - b.Id;

    private readonly List<IOrderedVariable> _window = rangeToCheck.ToList();
    
    protected override bool IsSatisfiableInternal(IDictionary<IOrderedVariable, IDomain<string>> domains)
    {
        // make sure we have a sort
        _window.Sort(_comparer);

        var ownerOptions = domains[owner].Values.ToList();
        foreach (var candidate in ownerOptions)
        {
            var firstQId = GetChoice(candidate);
            var valueToCheck = isDeterminedByOwner ? candidate : expected;
            if (firstQId == null)
            {
                // check that every Q can be assigned to not expected
                if (_window.All(q => domains[q].Values.Any(o => o != valueToCheck)))
                {
                    // found one
                    return true;
                }

                // not supported
                continue;
            }
            
            // can firstQId be the first?
            var targetIdx = _window.FindIndex(q => q.Id == firstQId);
            var targetQ = _window[targetIdx];

            if (!domains[targetQ].Values.Contains(valueToCheck!))
            {
                continue; // not supported if it can't even be that option
            }
            
            // now it is supported if there isn't a forced expected before it.
            var supported = true;
            for (var i = 0; i < targetIdx; i++)
            {
                var qi =  _window[i];
                var qiDomain = domains[qi].Values.ToList();
                if (qiDomain.Count == 1 && qiDomain[0] == valueToCheck!)
                {
                    supported = false;
                    break;
                }
            }

            if (supported)
            {
                return true;
            }
        }

        return false;
    }
}