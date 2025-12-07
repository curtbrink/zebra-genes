using Csp.Objects.Constraints.Impl.Selfref;
using Csp.Objects.Constraints.Interfaces;
using Csp.Objects.Domain;
using Csp.Objects.Variables.Interfaces;

namespace Csp.Builders.Quiz;

public class
    CustomConstraintQuestionBuilder<TParent> : QuestionBuilder<string, TParent,
    CustomConstraintQuestionBuilder<TParent>> where TParent : QuestionListBuilder<TParent>
{
    private readonly Func<IOrderedVariable, IDictionary<IOrderedVariable, IDomain<string>>, bool> _truthFunc;

    internal CustomConstraintQuestionBuilder(TParent qb, int choiceCount, int questionId,
        Func<IOrderedVariable, IDictionary<IOrderedVariable, IDomain<string>>, bool> truthFunc) : base(qb, choiceCount,
        questionId)
    {
        _truthFunc = truthFunc;
    }

    internal override IConstraint<string> BuildConstraint(IOrderedVariable me, List<IOrderedVariable> variables)
    {
        return new CustomFuncConstraint(me, variables, _truthFunc);
    }

    internal override void Validate()
    {
        // do I have to? I trust myself
        return;
    }
}