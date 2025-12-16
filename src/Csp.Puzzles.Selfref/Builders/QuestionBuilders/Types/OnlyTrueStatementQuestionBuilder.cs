using Csp.Core.Models.Models.Constraint.Interfaces;
using Csp.Core.Models.Models.Variable.Interfaces;
using Csp.Puzzles.Selfref.Constraints;

namespace Csp.Puzzles.Selfref.Builders;

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

    internal override IConstraint<string> BuildConstraint(IOrderedVariable me, List<IOrderedVariable> variables)
    {
        if (_child == null) throw new InvalidOperationException("Somehow I don't have a list builder.");
        // need an OnlyTrueStatementConstraint
        var childConstraints = _child.Questions.Select(q => (BaseSelfRefConstraint)q.BuildConstraint(me, variables))
            .ToList();
        foreach (var childConstraint in childConstraints)
        {
            childConstraint.OverrideSingle = true;
        }

        return new OnlyTrueStatementConstraint(me, variables, childConstraints);
    }

    internal override void Validate()
    {
        // do I have to? I trust myself
        return;
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