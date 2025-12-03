using Csp.Impl.Constraints.Selfref;
using Csp.Interfaces;

namespace Csp.Builders.Quiz;

public class
    OnlySameChoiceQuestionBuilder<TParent> : QuestionBuilder<int, OnlySameChoiceQuestionBuilder<TParent>, TParent>
    where TParent : QuestionListBuilder<TParent>
{
    internal OnlySameChoiceQuestionBuilder(TParent qb, int choiceCount, int questionId) : base(qb, choiceCount,
        questionId)
    {
    }

    internal override IConstraint<string> BuildConstraint(List<IOrderedVariable> variables) =>
        new OnlySameChoiceConstraint(GetMe(variables), variables, Choices);

    internal override void Validate(int minQ, int maxQ, List<string> domain, bool shouldValidate = true)
    {
        var badChoices = Choices.Where(c => c < 1 || c > Builder.MaxQuestionId).ToList();
        if (badChoices.Count > 0)
        {
            throw new Exception(
                $"Choices {{{string.Join(",", badChoices)}}} must be between 1 and {Builder.MaxQuestionId}, inclusive");
        }

        ValidateChoices();
    }
}