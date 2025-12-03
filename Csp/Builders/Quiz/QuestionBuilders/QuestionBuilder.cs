using Csp.Interfaces;

namespace Csp.Builders.Quiz;

public abstract class QuestionBuilder<TParent> : SelfRefBuilder<QuestionBuilder<TParent>>
    where TParent : QuestionListBuilder<TParent>
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

    internal abstract IConstraint<string> BuildConstraint(List<IOrderedVariable> variables);
    internal abstract void Validate(int minQ, int maxQ, List<string> domain, bool shouldValidate = true);

    public TParent EndQuestion() => Builder;

    protected IOrderedVariable GetMe(List<IOrderedVariable> variables) =>
        variables.First(v => v.Id == QuestionId);
}