namespace Csp.Builders.Quiz;

public abstract class QuestionBuilder<T, TMe, TParent> : QuestionBuilder<TParent>
    where TMe : QuestionBuilder<T, TMe, TParent>
    where TParent : QuestionListBuilder<TParent>
{
    protected readonly List<T> Choices = [];

    internal QuestionBuilder(TParent qb, int choiceCount, int questionId) : base(qb, choiceCount, questionId)
    {
    }

    public TMe WithChoices(params T[] choices)
    {
        Choices.AddRange(choices);
        return (TMe)this;
    }

    protected void ValidateChoices()
    {
        if (Choices.Count != ChoiceCount)
        {
            throw new Exception($"Number of choices must be {ChoiceCount} - got {Choices.Count}");
        }

        if (Choices.Count != Choices.Distinct().Count())
        {
            throw new Exception("Choices must all be unique");
        }
    }
}