using Csp.Impl.Constraints.Selfref;
using Csp.Interfaces;

namespace Csp.Builders.Quiz;

public class
    MostLeastCommonQuestionBuilder<TParent> : QuestionBuilder<string?, MostLeastCommonQuestionBuilder<TParent>, TParent>
    where TParent : QuestionListBuilder<TParent>
{
    private readonly bool _isLeast;

    internal MostLeastCommonQuestionBuilder(TParent qb, int choiceCount, int questionId, bool isLeast) :
        base(qb, choiceCount, questionId)
    {
        _isLeast = isLeast;
    }

    internal override IConstraint<string> BuildConstraint(List<IOrderedVariable> variables) =>
        new MostLeastCommonConstraint(GetMe(variables), variables, Choices, _isLeast);

    internal override void Validate(int minQ, int maxQ, List<string> domain, bool shouldValidate = true)
    {
        var badChoices = Choices.Where(c => c != null && !domain.Contains(c)).ToList();
        if (badChoices.Count > 0)
        {
            throw new Exception(
                $"Choices {{{string.Join(",", badChoices)}}} are not in domain {{{string.Join(",", domain)}}}");
        }

        ValidateChoices();
    }
}