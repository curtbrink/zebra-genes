using Csp.Core.Models.Models.Variable.Interfaces;

namespace Csp.Puzzles.Selfref.Constraints;

public abstract class BaseSelfRefConstraint<T> : BaseSelfRefConstraint
{
    private readonly IDictionary<string, T> _choiceList;

    protected string OptionString => string.Join("|", OptionListItems);

    private List<string> OptionListItems =>
        _choiceList.Keys.Select(k => $"{k}={(_choiceList[k] is null ? "None" : _choiceList[k])}").ToList();
    
    public override IReadOnlyList<IVariable> Scope => QuestionScope;
    public IReadOnlyList<IOrderedVariable> QuestionScope { get; }

    protected BaseSelfRefConstraint(IEnumerable<IOrderedVariable> scope, IEnumerable<T> choiceList)
    {
        QuestionScope = scope.ToList();
        _choiceList = new Dictionary<string, T>();
        var cs = choiceList.ToList();
        for (var i = 0; i < cs.Count; i++)
        {
            _choiceList[Options[i]] = cs[i];
        }
    }

    protected T GetChoice(string o)
    {
        var key = OverrideSingle ? "A" : o;
        return _choiceList[key];
    }
}