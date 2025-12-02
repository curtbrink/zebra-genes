using Csp.Impl.Constraints.Selfref;
using Csp.Interfaces;

namespace Csp.Builders.Quiz;

public partial class QuizBuilder
{
    public class MostCommonCountQuestionBuilder : QuestionBuilder<int, MostCommonCountQuestionBuilder>
    {
        internal MostCommonCountQuestionBuilder(QuizBuilder qb, int choiceCount, int questionId) :
            base(qb, choiceCount, questionId)
        {
        }

        internal override IConstraint<string> BuildConstraint(List<IOrderedVariable> variables) =>
            new MostLeastCommonConstraint(GetMe(variables), variables, Choices.Select(c => c.ToString()).ToList(),
                false, true);

        internal override void Validate()
        {
            var maxQuestionId = Builder._nextQuestionId - 1;
            var badChoices = Choices.Where(choice => choice > maxQuestionId || choice < 0).ToList();
            if (badChoices.Count > 0)
            {
                throw new Exception(
                    $"Choices {{{string.Join(",", badChoices)}}} must be between 0 and the question count {maxQuestionId}, inclusive");
            }

            ValidateChoices();
        }
    }
}