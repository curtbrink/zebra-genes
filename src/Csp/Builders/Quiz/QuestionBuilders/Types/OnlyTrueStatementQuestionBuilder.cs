using Csp.Impl.Constraints.Selfref;
using Csp.Interfaces;

namespace Csp.Builders.Quiz;

public class
    OnlyTrueStatementQuestionBuilder<TParent> : QuestionBuilder<BaseSelfRefConstraint, TParent,
    OnlyTrueStatementQuestionBuilder<TParent>> where TParent : QuestionListBuilder<TParent>
{
    private OnlyTrueStatementListBuilder? _child;

    private OnlyTrueStatementQuestionBuilder(TParent qb, int choiceCount, int questionId) : base(qb,
        choiceCount, questionId)
    {
    }

    private void SetListBuilder(OnlyTrueStatementListBuilder child)
    {
        _child = child;
    }

    public static OnlyTrueStatementListBuilder New(TParent qb, int choiceCount, int questionId)
    {
        if (typeof(TParent) != typeof(QuizBuilder))
            throw new ArgumentOutOfRangeException(nameof(qb), "Only true statement questions cannot be nested.");

        var questionBuilder = new OnlyTrueStatementQuestionBuilder<TParent>(qb, choiceCount, questionId);
        var listBuilder = new OnlyTrueStatementListBuilder(questionBuilder);
        questionBuilder.SetListBuilder(listBuilder);
        return listBuilder;
    }

    internal override IConstraint<string> BuildConstraint(List<IOrderedVariable> variables)
    {
        throw new NotImplementedException();
    }

    internal override void Validate(int minQ, int maxQ, List<string> domain, bool shouldValidate = true)
    {
        throw new NotImplementedException();
    }

    public class OnlyTrueStatementListBuilder : QuestionListBuilder<OnlyTrueStatementListBuilder>
    {
        internal readonly OnlyTrueStatementQuestionBuilder<TParent> Parent;

        internal OnlyTrueStatementListBuilder(OnlyTrueStatementQuestionBuilder<TParent> questionBuilder) : base(1)
        {
            Parent = questionBuilder;
        }

        public QuestionBuilderInit<OnlyTrueStatementListBuilder> WhatIsThe() => new(this, 1, 1);

        public OnlyTrueStatementQuestionBuilder<TParent> Finished() => Parent;

        internal override int MinQuestionId => 1;
        internal override int MaxQuestionId => 1;
    }
}