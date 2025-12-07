using Csp.Objects.Domain;
using Csp.Objects.Variables.Interfaces;

namespace Csp.Builders.Quiz;

public class QuestionBuilderInit<TParent>(TParent qb, int choiceCount, int questionId)
    where TParent : QuestionListBuilder<TParent>
{
    private void Add(QuestionBuilder<TParent> questionBuilder)
    {
        qb.Questions.Add(questionBuilder);
    }

    // ==== DIRECT MAP QUESTIONS ====
    public MapChoiceQuestionBuilder<TParent> AnswerToQuestion(int otherQuestionId)
    {
        var b = new MapChoiceQuestionBuilder<TParent>(qb, choiceCount, questionId, otherQuestionId);
        Add(b);
        return b;
    }

    // ==== COUNT OF ANSWER QUESTIONS ====
    public CountOfChoiceQuestionBuilder<TParent> CountOfAnswer(string answer) => CountOfChoices([answer]);

    public CountOfChoiceQuestionBuilder<TParent> CountOfVowels() => CountOfChoices(["A", "E"]);

    public CountOfChoiceQuestionBuilder<TParent> CountOfConsonants() => CountOfChoices(["B", "C", "D"]);

    private CountOfChoiceQuestionBuilder<TParent> CountOfChoices(List<string> choices)
    {
        var b = new CountOfChoiceQuestionBuilder<TParent>(qb, choiceCount, questionId, choices);
        Add(b);
        return b;
    }

    // ==== FIRST QUESTION WITH ANSWER QUESTIONS ====
    public FirstWithChoiceQuestionBuilder<TParent> First() => FirstWithChoice(false, false);

    public FirstWithChoiceQuestionBuilder<TParent> Last() => FirstWithChoice(true, false);

    public FirstWithChoiceQuestionBuilder<TParent> Next() => FirstWithChoice(false, true);

    public FirstWithChoiceQuestionBuilder<TParent> Previous() => FirstWithChoice(true, true);

    private FirstWithChoiceQuestionBuilder<TParent> FirstWithChoice(bool isReverse, bool isNext)
    {
        var b = new FirstWithChoiceQuestionBuilder<TParent>(qb, choiceCount, questionId, isReverse, isNext);
        Add(b);
        return b;
    }

    // ==== MOST/LEAST COMMON/COUNT OF MOST COMMON QUESTIONS ====
    public MostLeastCommonQuestionBuilder<TParent> MostCommonAnswer()
    {
        var b = new MostLeastCommonQuestionBuilder<TParent>(qb, choiceCount, questionId, false);
        Add(b);
        return b;
    }

    public MostLeastCommonQuestionBuilder<TParent> LeastCommonAnswer()
    {
        var b = new MostLeastCommonQuestionBuilder<TParent>(qb, choiceCount, questionId, true);
        Add(b);
        return b;
    }

    public MostCommonCountQuestionBuilder<TParent> CountOfMostCommonAnswer()
    {
        var b = new MostCommonCountQuestionBuilder<TParent>(qb, choiceCount, questionId);
        Add(b);
        return b;
    }

    // ==== ONLY CONSECUTIVE N QUESTIONS ====
    public OnlyConsecutiveSameQuestionBuilder<TParent> OnlyConsecutiveSameSetOf(int n)
    {
        var b = new OnlyConsecutiveSameQuestionBuilder<TParent>(qb, choiceCount, questionId, n);
        Add(b);
        return b;
    }

    // ==== ONLY QUESTION WITH SAME ANSWER QUESTIONS ==== 
    public OnlySameChoiceQuestionBuilder<TParent> OnlyQuestionWithTheSameAnswer()
    {
        var b = new OnlySameChoiceQuestionBuilder<TParent>(qb, choiceCount, questionId);
        Add(b);
        return b;
    }
    
    // ==== ONLY TRUE STATEMENT QUESTIONS ====
    public OnlyTrueStatementQuestionBuilder<TParent>.OnlyTrueStatementListBuilder OnlyTrueStatement()
    {
        // slightly different since we get a list builder
        var lb = OnlyTrueStatementQuestionBuilder<TParent>.New(qb, choiceCount, questionId);
        Add(lb.Parent); // but we can still access the wrapper question builder
        return lb;
    }
    
    // ==== CUSTOM CONSTRAINT ====
    public CustomConstraintQuestionBuilder<TParent> CustomConstraint(
        Func<IOrderedVariable, IDictionary<IOrderedVariable, IDomain<string>>, bool> truthFunc)
    {
        var b = new CustomConstraintQuestionBuilder<TParent>(qb, choiceCount, questionId, truthFunc);
        Add(b);
        return b;
    }
}