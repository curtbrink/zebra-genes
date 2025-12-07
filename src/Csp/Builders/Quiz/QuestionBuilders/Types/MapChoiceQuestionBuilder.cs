using Csp.Impl.Constraints.Selfref;
using Csp.Interfaces;
using Csp.Objects.Variables.Interfaces;

namespace Csp.Builders.Quiz;

public class MapChoiceQuestionBuilder<TParent> : QuestionBuilder<string, TParent, MapChoiceQuestionBuilder<TParent>>
    where TParent : QuestionListBuilder<TParent>
{
    private readonly int _mapToQuestionId;

    internal MapChoiceQuestionBuilder(TParent qb, int choiceCount, int questionId, int otherQuestionId) : base(
        qb, choiceCount, questionId)
    {
        _mapToQuestionId = otherQuestionId;
    }

    internal override IConstraint<string> BuildConstraint(IOrderedVariable me, List<IOrderedVariable> variables)
    {
        var other = variables.First(v => v.Id == _mapToQuestionId);
        return new ChoiceEqualsConstraint(me, other, Choices);
    }

    internal override void Validate()
    {
        if (_mapToQuestionId < Builder.MinQuestionId || _mapToQuestionId > Builder.MaxQuestionId)
        {
            throw new Exception($"Question #{_mapToQuestionId} not found");
        }
        var badChoices = Choices.Where(c => !Builder.Domain.Values.Contains(c)).ToList();
        if (badChoices.Count > 0)
        {
            throw new Exception(
                $"Choices {{{string.Join(",", badChoices)}}} are not in domain {{{string.Join(",", Builder.Domain.Values)}}}");
        }

        ValidateChoices();
    }
}