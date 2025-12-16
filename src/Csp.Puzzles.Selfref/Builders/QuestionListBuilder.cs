using Csp.Core.Models.Models.Domain;

namespace Csp.Puzzles.Selfref.Builders;

public abstract class QuestionListBuilder<TSelf> : SelfRefBuilder<TSelf>
    where TSelf : QuestionListBuilder<TSelf>
{
    private readonly List<string> _options =
        ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T"];

    internal readonly ImmutableDomain<string> Domain;
    internal readonly int ChoiceCount;
    internal readonly List<QuestionBuilder<TSelf>> Questions = [];

    internal abstract int MinQuestionId { get; }
    internal abstract int MaxQuestionId { get; }

    protected QuestionListBuilder(int choiceCount)
    {
        ChoiceCount = choiceCount;
        Domain = new ImmutableDomain<string>(_options[..choiceCount]);
    }
}