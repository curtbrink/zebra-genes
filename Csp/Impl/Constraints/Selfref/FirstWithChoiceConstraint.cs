using Csp.Interfaces;

namespace Csp.Impl.Constraints.Selfref;

// this type can cover all of these:
// first X
// last X -> same as first X if the questions were reversed
// closest X after #Y -> scope is a narrower subset of Qs
// closest X before #Y -> same as closest after, with same narrower scope and questions reversed

public class FirstWithChoiceConstraint(
    IEnumerable<IOrderedVariable> scope,
    IOrderedVariable owner,
    string expected,
    IEnumerable<int?> choiceList,
    bool isReverse = false) : BaseSelfRefConstraint<int?>(scope, choiceList)
{
    public override string Name => "FirstWithChoice";
    public override string Description => $"First Q with choice {expected} is {owner.Name}=>{{{OptionString}}}";

    private readonly Comparison<IOrderedVariable> _comparer = isReverse ? (a, b) => b.Id - a.Id : (a, b) => a.Id - b.Id;

    
    public override bool IsSatisfied(IAssignment<string> assignment)
    {
        if (!assignment.IsAssigned(owner))
        {
            return false;
        }

        var firstQWithChoice = ChoiceList[assignment.GetValue(owner)];
        
        // make sure we sort just in case
        var sortedVs = scope.ToList();
        sortedVs.Sort(_comparer);

        foreach (var v in sortedVs)
        {
            // did we go too far?
            if (firstQWithChoice is not null && (isReverse ? v.Id < firstQWithChoice : v.Id > firstQWithChoice))
            {
                return false;
            }
            
            if (assignment.IsAssigned(v))
            {
                if (assignment.GetValue(v) == expected)
                {
                    return v.Id == firstQWithChoice;
                }
            }
        }

        // didn't find an expected choice!
        return firstQWithChoice is null;
    }
}