using Csp.Interfaces;

namespace Csp.Impl.Constraints.Selfref;

// represents a generic logiquiz-type question constraint with ABCDE values mapped to something
public abstract class BaseSelfRefConstraint<T> : IConstraint<string>
{
    protected readonly IList<string> Options = ["A", "B", "C", "D", "E"];
    protected readonly IDictionary<string, T> ChoiceList;
    
    protected string OptionString => string.Join("|", ChoiceList.Keys.Select(k => $"{k}={ChoiceList[k]}"));

    
    public abstract string Name { get; }
    public abstract string Description { get; }
    public IReadOnlyList<IVariable> Scope { get; }

    protected BaseSelfRefConstraint(IEnumerable<IVariable> scope, IEnumerable<T> choiceList)
    {
        Scope = scope.ToList();
        ChoiceList = new Dictionary<string, T>();
        var cs = choiceList.ToList();
        for (var i = 0; i < cs.Count; i++)
        {
            ChoiceList[Options[i]] = cs[i];
        }
    }

    public abstract bool IsSatisfied(IAssignment<string> assignment);
}