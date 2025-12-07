using Csp.Objects.Constraints.Interfaces;
using Csp.Objects.Csp;
using Csp.Objects.Variables.Impl;
using Csp.Objects.Variables.Interfaces;

namespace Csp.Builders.Quiz;

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

    public UniformDomainCsp<string> Build()
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
        return new UniformDomainCsp<string>(variables, Domain, constraints);
    }
}