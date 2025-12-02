using Csp.Impl.Constraints.Selfref;
using Csp.Interfaces;

namespace Csp.Builders.Quiz;

public partial class QuizBuilder
{
    public class OnlySameChoiceQuestionBuilder : QuestionBuilder<int, OnlySameChoiceQuestionBuilder>
    {
        internal OnlySameChoiceQuestionBuilder(QuizBuilder qb, int choiceCount, int questionId) : base(qb, choiceCount,
            questionId)
        {
        }

        internal override IConstraint<string> BuildConstraint(List<IOrderedVariable> variables) =>
            new OnlySameChoiceConstraint(GetMe(variables), variables, Choices);

        internal override void Validate()
        {
            var badChoices = Choices.Where(c => c < 1 || c >= Builder._nextQuestionId).ToList();
            if (badChoices.Count > 0)
            {
                throw new Exception(
                    $"Choices {{{string.Join(",", badChoices)}}} must be between 1 and {Builder._nextQuestionId - 1}, inclusive");
            }

            ValidateChoices();
        }
    }
}