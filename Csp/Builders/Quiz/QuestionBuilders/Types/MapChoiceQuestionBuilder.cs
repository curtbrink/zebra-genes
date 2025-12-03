using Csp.Impl.Constraints.Selfref;
using Csp.Interfaces;

namespace Csp.Builders.Quiz;

public class MapChoiceQuestionBuilder<TParent> : QuestionBuilder<string, MapChoiceQuestionBuilder<TParent>, TParent>
    where TParent : QuestionListBuilder<TParent>
{
    private readonly int _mapToQuestionId;

    internal MapChoiceQuestionBuilder(TParent qb, int choiceCount, int questionId, int otherQuestionId) : base(
        qb, choiceCount, questionId)
    {
        _mapToQuestionId = otherQuestionId;
    }

    internal override IConstraint<string> BuildConstraint(List<IOrderedVariable> variables)
    {
        var other = variables.First(v => v.Id == _mapToQuestionId);
        return new ChoiceEqualsConstraint(GetMe(variables), other, Choices);
    }

    internal override void Validate(int minQ, int maxQ, List<string> domain, bool shouldValidate = true)
    {
        if (!shouldValidate) return;
        
        if (_mapToQuestionId < minQ || _mapToQuestionId > maxQ)
        {
            throw new Exception($"Question #{_mapToQuestionId} not found");
        }
        var badChoices = Choices.Where(c => !domain.Contains(c)).ToList();
        if (badChoices.Count > 0)
        {
            throw new Exception(
                $"Choices {{{string.Join(",", badChoices)}}} are not in domain {{{string.Join(",", domain)}}}");
        }

        ValidateChoices();
    }
}