using Csp.Core.Models.Models.Constraint.Interfaces;
using Csp.Core.Models.Models.Domain.Interfaces;
using Csp.Core.Models.Models.Variable.Interfaces;
using Csp.Puzzles.Selfref.Constraints;

namespace Csp.Puzzles.Selfref.Builders;

public class
    CustomConstraintQuestionBuilder<TParent> : QuestionBuilder<string, TParent,
    CustomConstraintQuestionBuilder<TParent>> where TParent : QuestionListBuilder<TParent>
{
    private readonly Func<IOrderedVariable, IDomainStore<string>, bool> _truthFunc;

    internal CustomConstraintQuestionBuilder(TParent qb, int choiceCount, int questionId,
        Func<IOrderedVariable, IDomainStore<string>, bool> truthFunc) : base(qb, choiceCount,
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