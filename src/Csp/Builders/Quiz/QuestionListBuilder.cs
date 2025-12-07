using Csp.Objects.Domain;

namespace Csp.Builders.Quiz;

public abstract class QuestionListBuilder<TSelf> : SelfRefBuilder<TSelf> 
    where TSelf : QuestionListBuilder<TSelf>
{
    private readonly List<string> _options =
        ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T"];
    
    internal readonly Domain<string> Domain;
    internal readonly int ChoiceCount;
    internal readonly List<QuestionBuilder<TSelf>> Questions = [];

    internal abstract int MinQuestionId { get; }
    internal abstract int MaxQuestionId { get; }

    protected QuestionListBuilder(int choiceCount)
    {
        ChoiceCount = choiceCount;
        Domain = new Domain<string>(_options[0..choiceCount]);
    }
}