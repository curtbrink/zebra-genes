using Csp.Impl.Constraints.Selfref;
using Csp.Interfaces;

namespace Csp.Builders.Quiz;

public partial class QuizBuilder
{
    public class CountOfChoiceQuestionBuilder : QuestionBuilder<int, CountOfChoiceQuestionBuilder>
    {
        private readonly List<string> _choicesToCount;
        
        private int? _threshold;
        private bool _isAfter;
        
        private int MaxQuestionId =>
            _threshold == null || _isAfter ? Builder._nextQuestionId - 1 : _threshold.Value - 1;

        private int MinQuestionId => _threshold == null || !_isAfter ? 1 : _threshold.Value + 1;

        internal CountOfChoiceQuestionBuilder(QuizBuilder qb, int choiceCount, int questionId,
            List<string> choicesToCount) : base(qb, choiceCount, questionId)
        {
            _choicesToCount = choicesToCount;
        }
        
        public CountOfChoiceQuestionBuilder Before(int threshold)
        {
            _isAfter = false;
            _threshold = threshold;
            return this;
        }

        public CountOfChoiceQuestionBuilder After(int threshold)
        {
            _isAfter = true;
            _threshold = threshold;
            return this;
        }

        internal override IConstraint<string> BuildConstraint(List<IOrderedVariable> variables)
        {
            // what's my range?
            var rangeToCheck = new List<IOrderedVariable>();
            for (var i = MinQuestionId; i <= MaxQuestionId; i++)
            {
                rangeToCheck.Add(variables.First(v => v.Id == i));
            }
            
            return new AnswerCountConstraint(GetMe(variables), rangeToCheck, _choicesToCount, Choices);
        }

        internal override void Validate()
        {
            var domain = Builder.Domain.Values.ToList();
            if (!_choicesToCount.All(co => domain.Contains(co)))
            {
                throw new Exception($"Answers to count contains a value not in domain {{{string.Join(",", domain)}}}");
            }

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