using Csp.Objects.Constraints.Impl.Selfref;
using Csp.Objects.Constraints.Interfaces;
using Csp.Objects.Variables.Interfaces;

namespace Csp.Builders.Quiz;

public class
    OnlyConsecutiveSameQuestionBuilder<TParent> : QuestionBuilder<int, TParent,
    OnlyConsecutiveSameQuestionBuilder<TParent>> where TParent : QuestionListBuilder<TParent>
{
    private readonly int _consecutiveSameSize;

    internal OnlyConsecutiveSameQuestionBuilder(TParent qb, int choiceCount, int questionId,
        int consecutiveSame) : base(qb, choiceCount, questionId)
    {
        _consecutiveSameSize = consecutiveSame;
    }

    internal override IConstraint<string> BuildConstraint(IOrderedVariable me, List<IOrderedVariable> variables)
    {
        // need to convert "starting ids" to the actual window
        var windows = Choices.Select(c =>
        {
            var window = new List<int>();
            for (var i = 0; i < _consecutiveSameSize; i++)
            {
                window.Add(c + i);
            }

            return window;
        }).ToList();
        return new OnlyConsecutiveSameConstraint(me, variables, windows);
    }

    internal override void Validate()
    {
        // if 8 questions and our set size is 3, the highest one can be 6 -> checking 6-8
        // if 8 questions, the next id at this point is 9, so 9-3 = 6.
        var maxQuestionId = Builder.MaxQuestionId - _consecutiveSameSize + 1;
        var badChoices = Choices.Where(c => c < 1 || c > maxQuestionId).ToList();
        if (badChoices.Count > 0)
        {
            throw new Exception(
                $"Choices {{{string.Join(",", badChoices)}}} must be between 1 and {maxQuestionId}, inclusive");
        }

        ValidateChoices();
    }
}