using Csp.Impl.Constraints.Selfref;
using Csp.Interfaces;

namespace Csp.Builders.Quiz;

public partial class QuizBuilder
{
    public class MapChoiceQuestionBuilder : QuestionBuilder<string, MapChoiceQuestionBuilder>
    {
        private readonly int _mapToQuestionId;

        internal MapChoiceQuestionBuilder(QuizBuilder qb, int choiceCount, int questionId, int otherQuestionId) : base(
            qb, choiceCount, questionId)
        {
            _mapToQuestionId = otherQuestionId;
        }

        internal override IConstraint<string> BuildConstraint(List<IOrderedVariable> variables)
        {
            var other = variables.First(v => v.Id == _mapToQuestionId);
            return new ChoiceEqualsConstraint(GetMe(variables), other, Choices);
        }

        internal override void Validate()
        {
            if (Builder._nextQuestionId <= _mapToQuestionId)
            {
                throw new Exception($"Question #{_mapToQuestionId} not found");
            }

            var domain = Builder.Domain.Values.ToList();
            var badChoices = Choices.Where(c => !Builder.Domain.Values.Contains(c)).ToList();
            if (badChoices.Count > 0)
            {
                throw new Exception(
                    $"Choices {{{string.Join(",", badChoices)}}} are not in domain {{{string.Join(",", domain)}}}");
            }

            ValidateChoices();
        }
    }
}