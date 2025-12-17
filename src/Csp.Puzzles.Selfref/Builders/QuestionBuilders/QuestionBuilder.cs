using Csp.Core.Models.Models.Constraint.Interfaces;
using Csp.Core.Models.Models.Variable.Interfaces;

namespace Csp.Puzzles.Selfref.Builders;

public abstract class QuestionBuilder<TParent> where TParent : QuestionListBuilder<TParent>
{
    internal int QuestionId;
    protected TParent Builder;
    protected int ChoiceCount;

    internal QuestionBuilder(TParent qb, int choiceCount, int questionId)
    {
        Builder = qb;
        ChoiceCount = choiceCount;
        QuestionId = questionId;
    }

    internal abstract IConstraint<string> BuildConstraint(IOrderedVariable me, List<IOrderedVariable> variables);
    internal abstract void Validate();

    public TParent EndQuestion() => Builder;
}