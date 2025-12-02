namespace Csp.Builders.Quiz;

public partial class QuizBuilder
{
    public abstract class QuestionBuilder<T, TBuild> : QuestionBuilder
        where TBuild : QuestionBuilder<T, TBuild>
    {
        protected readonly List<T> Choices = [];

        internal QuestionBuilder(QuizBuilder qb, int choiceCount, int questionId) : base(qb, choiceCount, questionId)
        {
        }

        public TBuild WithChoices(params T[] choices)
        {
            Choices.AddRange(choices);
            return (TBuild)this;
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
}