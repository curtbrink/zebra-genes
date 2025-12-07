using Csp.Objects.Constraints.Impl.Selfref;
using Csp.Objects.Constraints.Interfaces;
using Csp.Objects.Variables.Interfaces;

namespace Csp.Builders.Quiz;

public class
    MostLeastCommonQuestionBuilder<TParent> : QuestionBuilder<string?, TParent, MostLeastCommonQuestionBuilder<TParent>>
    where TParent : QuestionListBuilder<TParent>
{
    private readonly bool _isLeast;

    internal MostLeastCommonQuestionBuilder(TParent qb, int choiceCount, int questionId, bool isLeast) :
        base(qb, choiceCount, questionId)
    {
        _isLeast = isLeast;
    }

    internal override IConstraint<string> BuildConstraint(IOrderedVariable me, List<IOrderedVariable> variables) =>
        new MostLeastCommonConstraint(me, variables, Choices, _isLeast);

    internal override void Validate()
    {
        var badChoices = Choices.Where(c => c != null && !Builder.Domain.Values.Contains(c)).ToList();
        if (badChoices.Count > 0)
        {
            throw new Exception(
                $"Choices {{{string.Join(",", badChoices)}}} are not in domain {{{string.Join(",", Builder.Domain.Values)}}}");
        }

        ValidateChoices();
    }
}