using Csp.Objects.Constraints.Impl.Selfref;
using Csp.Objects.Constraints.Interfaces;
using Csp.Objects.Variables.Interfaces;

namespace Csp.Builders.Quiz;

public class
    OnlySameChoiceQuestionBuilder<TParent> : QuestionBuilder<int, TParent, OnlySameChoiceQuestionBuilder<TParent>>
    where TParent : QuestionListBuilder<TParent>
{
    internal OnlySameChoiceQuestionBuilder(TParent qb, int choiceCount, int questionId) : base(qb, choiceCount,
        questionId)
    {
    }

    internal override IConstraint<string> BuildConstraint(IOrderedVariable me, List<IOrderedVariable> variables) =>
        new OnlySameChoiceConstraint(me, variables, Choices);

    internal override void Validate()
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