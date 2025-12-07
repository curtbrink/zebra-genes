using Csp.Interfaces;
using Csp.Objects.Variables.Interfaces;

namespace Csp.Builders.Quiz;

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