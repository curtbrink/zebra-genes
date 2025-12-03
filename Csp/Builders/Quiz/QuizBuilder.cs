using Csp.Impl;
using Csp.Interfaces;

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
            qb.Validate(1, _nextQuestionId - 1, Domain.Values.ToList());
            constraints.Add(qb.BuildConstraint(variables));
        }

        // build csp
        return new UniformDomainCsp<string>(variables, Domain, constraints);
    }
}