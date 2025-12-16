using Csp.Core.Models.Models.Constraint.Interfaces;
using Csp.Core.Models.Models.Variable.Interfaces;
using Csp.Puzzles.Selfref.Constraints;

namespace Csp.Puzzles.Selfref.Builders;

public class
    MostCommonCountQuestionBuilder<TParent> : QuestionBuilder<int, TParent, MostCommonCountQuestionBuilder<TParent>>
    where TParent : QuestionListBuilder<TParent>
{
    internal MostCommonCountQuestionBuilder(TParent qb, int choiceCount, int questionId) :
        base(qb, choiceCount, questionId)
    {
    }

    internal override IConstraint<string> BuildConstraint(IOrderedVariable me, List<IOrderedVariable> variables) =>
        new MostLeastCommonConstraint(me, variables, Choices.Select(c => c.ToString()).ToList(),
            false, true);

    internal override void Validate()
    {
        var badChoices = Choices.Where(choice => choice > Builder.MaxQuestionId || choice < 0).ToList();
        if (badChoices.Count > 0)
        {
            throw new Exception(
                $"Choices {{{string.Join(",", badChoices)}}} must be between 0 and the question count {Builder.MaxQuestionId}, inclusive");
        }

        ValidateChoices();
    }
}