using Csp.Objects.Constraints.Impl.Selfref;
using Csp.Objects.Constraints.Interfaces;
using Csp.Objects.Variables.Interfaces;

namespace Csp.Builders.Quiz;

public class
    FirstWithChoiceQuestionBuilder<TParent> : QuestionBuilder<int?, TParent, FirstWithChoiceQuestionBuilder<TParent>>
    where TParent : QuestionListBuilder<TParent>
{
    private readonly bool _isReverse;

    private int? _threshold;
    private bool _isAfter;
    private bool _isDeterminedByOwner;
    private string? _choiceToCount;

    private int? MaxQuestionId =>
        _threshold == null || _isAfter ? null : _threshold.Value - 1;

    private int MinQuestionId => _threshold == null || !_isAfter ? 1 : _threshold.Value + 1;

    internal FirstWithChoiceQuestionBuilder(TParent qb, int choiceCount, int questionId, bool isReverse,
        bool isNext = false) : base(qb, choiceCount, questionId)
    {
        // first x => no threshold, not reverse
        // last x => no threshold, reverse
        // next x => threshold, not reverse, isAfter
        // prev x => threshold, reverse, isBefore
        _isReverse = isReverse;
        if (isNext)
        {
            _threshold = questionId;
            _isAfter = !isReverse;
        }
    }

    public FirstWithChoiceQuestionBuilder<TParent> Before(int threshold)
    {
        _isAfter = false;
        _threshold = threshold;
        return this;
    }

    public FirstWithChoiceQuestionBuilder<TParent> After(int threshold)
    {
        _isAfter = true;
        _threshold = threshold;
        return this;
    }

    public FirstWithChoiceQuestionBuilder<TParent> WithAnswer(string answer)
    {
        _isDeterminedByOwner = false;
        _choiceToCount = answer;
        return this;
    }

    public FirstWithChoiceQuestionBuilder<TParent> WithSameAnswer()
    {
        _isDeterminedByOwner = true;
        _choiceToCount = null;
        return this;
    }

    internal override IConstraint<string> BuildConstraint(IOrderedVariable me, List<IOrderedVariable> variables)
    {
        // what's my range?
        var rangeToCheck = new List<IOrderedVariable>();
        for (var i = MinQuestionId; i <= (MaxQuestionId ?? variables.Count); i++)
        {
            rangeToCheck.Add(variables.First(v => v.Id == i));
        }

        return new FirstWithChoiceConstraint(me, rangeToCheck, Choices, _choiceToCount, _isReverse,
            _isDeterminedByOwner);
    }

    internal override void Validate()
    {
        // check type of question
        if (_isDeterminedByOwner)
        {
            if (_choiceToCount != null) throw new Exception($"Same as this one question has a defined answer.");
        }
        else
        {
            if (_choiceToCount == null) throw new Exception($"First/last with answer has no answer to look for");
            if (!Builder.Domain.Values.Contains(_choiceToCount))
                throw new Exception(
                    $"Answer to find {_choiceToCount} not in domain {{{string.Join(",", Builder.Domain.Values)}}}");
        }

        // var badChoices = Choices
        //     .Where(choice => choice != null && (choice > MaxQuestionId || choice < MinQuestionId))
        //     .Select(v => v!.Value).ToList();
        // if (badChoices.Count > 0)
        // {
        //     throw new Exception(
        //         $"Choices {{{string.Join(",", badChoices)}}} must be between {MinQuestionId} and {MaxQuestionId}, inclusive");
        // }

        ValidateChoices();
    }
}