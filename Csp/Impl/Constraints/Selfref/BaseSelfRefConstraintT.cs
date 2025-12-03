using Csp.Helpers;
using Csp.Interfaces;

namespace Csp.Impl.Constraints.Selfref;

// represents a generic logiquiz-type question constraint with ABCDE values mapped to something
public abstract class BaseSelfRefConstraint<T> : BaseSelfRefConstraint
{
    protected readonly IDictionary<string, T> ChoiceList;

    protected string OptionString => string.Join("|", OptionListItems);

    private List<string> OptionListItems =>
        ChoiceList.Keys.Select(k => $"{k}={(ChoiceList[k] is null ? "None" : ChoiceList[k])}").ToList();
    
    public override IReadOnlyList<IVariable> Scope => QuestionScope;
    public IReadOnlyList<IOrderedVariable> QuestionScope { get; }

    protected BaseSelfRefConstraint(IEnumerable<IOrderedVariable> scope, IEnumerable<T> choiceList)
    {
        QuestionScope = scope.ToList();
        ChoiceList = new Dictionary<string, T>();
        var cs = choiceList.ToList();
        for (var i = 0; i < cs.Count; i++)
        {
            ChoiceList[Options[i]] = cs[i];
        }
    }
}