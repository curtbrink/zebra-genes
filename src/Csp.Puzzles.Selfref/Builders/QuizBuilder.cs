using Csp.Core.Models.Models.Constraint.Interfaces;
using Csp.Core.Models.Models.Csp;
using Csp.Core.Models.Models.Csp.Interfaces;
using Csp.Core.Models.Models.Variable;
using Csp.Core.Models.Models.Variable.Interfaces;

namespace Csp.Puzzles.Selfref.Builders;

public class QuizBuilder : QuestionListBuilder<QuizBuilder>
{
    private int _nextQuestionId = 1;

    internal override int MinQuestionId => 1;
    internal override int MaxQuestionId => _nextQuestionId - 1;

    private QuizBuilder(int choiceCount) : base(choiceCount)
    {
    }

    public static QuizBuilder New(int size) => new(size);

    public QuestionBuilderInit<QuizBuilder> WhatIsThe() => new(this, ChoiceCount, _nextQuestionId++);

    public ICsp<string> Build()
    {
        // validate each question and build its constraint
        List<IOrderedVariable> variables = [];
        variables.AddRange(Questions.Select(qb => new OrderedVariable($"Q{qb.QuestionId}", qb.QuestionId)).ToList());

        var constraints = new List<IConstraint<string>>();
        foreach (var qb in Questions)
        {
            qb.Validate();
            var me = variables.First(q => q.Id == qb.QuestionId);
            constraints.Add(qb.BuildConstraint(me, variables));
        }

        // build csp
        return new BaseCsp<string>(variables, Domain, constraints);
    }
}