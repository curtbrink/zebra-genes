using Csp.Impl;
using Csp.Interfaces;

namespace Csp.Builders.Quiz;

public partial class QuizBuilder
{
    private static List<string> _options =
        ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T"];

    private readonly int _choiceCount;
    internal readonly Domain<string> Domain;
    private int _nextQuestionId = 1;

    private readonly List<QuestionBuilder> _questions = [];

    private QuizBuilder(int choiceCount)
    {
        _choiceCount = choiceCount;
        Domain = new Domain<string>(_options[0..choiceCount]);
    }

    public static QuizBuilder New(int size) => new(size);

    public QuestionBuilderInit WhatIsThe() => new(this, _choiceCount, _nextQuestionId++);

    public UniformDomainCsp<string> Build()
    {
        // validate each question and build its constraint
        List<IOrderedVariable> variables = [];
        variables.AddRange(_questions.Select(qb => new OrderedVariable($"Q{qb.QuestionId}", qb.QuestionId)).ToList());

        var constraints = new List<IConstraint<string>>();
        foreach (var qb in _questions)
        {
            qb.Validate();
            constraints.Add(qb.BuildConstraint(variables));
        }

        // build csp
        return new UniformDomainCsp<string>(variables, Domain, constraints);
    }
}