namespace Csp.Builders.Quiz;

public abstract class QuestionBuilder<TData, TParent, TSelf> : QuestionBuilder<TParent>
    where TParent : QuestionListBuilder<TParent>
    where TSelf : QuestionBuilder<TData, TParent, TSelf>
{
    protected readonly List<TData> Choices = [];

    internal QuestionBuilder(TParent qb, int choiceCount, int questionId) : base(qb, choiceCount, questionId)
    {
    }

    public TSelf WithChoices(params TData[] choices)
    {
        Choices.AddRange(choices);
        return (TSelf)this;
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