using Csp.Impl;
using Csp.Impl.Constraints.Selfref;
using Csp.Interfaces;

namespace Csp.Builders;

public class QuizBuilder
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

    public class QuestionBuilderInit(QuizBuilder qb, int choiceCount, int questionId)
    {
        private void Add(QuestionBuilder questionBuilder)
        {
            qb._questions.Add(questionBuilder);
        }

        public MapChoiceQuestionBuilder AnswerToQuestion(int otherQuestionId)
        {
            var b = new MapChoiceQuestionBuilder(qb, choiceCount, questionId, otherQuestionId);
            Add(b);
            return b;
        }

        public CountOfChoiceQuestionBuilder CountOfAnswer(string answer)
        {
            var b = new CountOfChoiceQuestionBuilder(qb, choiceCount, questionId, answer);
            Add(b);
            return b;
        }

        public FirstWithChoiceQuestionBuilder First(string answer) => FirstWithChoice(answer, false);

        public FirstWithChoiceQuestionBuilder Last(string answer) => FirstWithChoice(answer, true);

        private FirstWithChoiceQuestionBuilder FirstWithChoice(string answer, bool isReverse)
        {
            var b = new FirstWithChoiceQuestionBuilder(qb, choiceCount, questionId, answer, isReverse);
            Add(b);
            return b;
        }

        public MostLeastCommonQuestionBuilder MostCommonAnswer()
        {
            var b = new MostLeastCommonQuestionBuilder(qb, choiceCount, questionId, false);
            Add(b);
            return b;
        }

        public MostLeastCommonQuestionBuilder LeastCommonAnswer()
        {
            var b = new MostLeastCommonQuestionBuilder(qb, choiceCount, questionId, true);
            Add(b);
            return b;
        }

        public OnlyConsecutiveSameQuestionBuilder OnlyConsecutiveSameSetOf(int n)
        {
            var b = new OnlyConsecutiveSameQuestionBuilder(qb, choiceCount, questionId, n);
            Add(b);
            return b;
        }

        public OnlySameChoiceQuestionBuilder OnlyQuestionWithTheSameAnswer()
        {
            var b = new OnlySameChoiceQuestionBuilder(qb, choiceCount, questionId);
            Add(b);
            return b;
        }
    }

    public abstract class QuestionBuilder(QuizBuilder qb, int choiceCount, int questionId)
    {
        internal int QuestionId = questionId;
        protected QuizBuilder Builder = qb;
        protected int ChoiceCount = choiceCount;

        internal abstract IConstraint<string> BuildConstraint(List<IOrderedVariable> variables);
        internal abstract void Validate();

        public QuizBuilder EndQuestion() => Builder;

        protected IOrderedVariable GetMe(List<IOrderedVariable> variables) =>
            variables.First(v => v.Id == QuestionId);
    }

    public abstract class QuestionBuilder<T, TBuild> : QuestionBuilder
        where TBuild : QuestionBuilder<T, TBuild>
    {
        protected readonly List<T> Choices = [];

        internal QuestionBuilder(QuizBuilder qb, int choiceCount, int questionId) : base(qb, choiceCount, questionId)
        {
        }

        public TBuild WithChoices(params T[] choices)
        {
            Choices.AddRange(choices);
            return (TBuild)this;
        }

        protected void ValidateChoices()
        {
            if (Choices.Count != ChoiceCount)
            {
                throw new Exception($"Number of choices must be {ChoiceCount} - got {Choices.Count}");
            }

            if (Choices.Count != Choices.Distinct().Count())
            {
                throw new Exception("Choices must all be unique");
            }
        }
    }

    public class MapChoiceQuestionBuilder : QuestionBuilder<string, MapChoiceQuestionBuilder>
    {
        private readonly int _mapToQuestionId;

        internal MapChoiceQuestionBuilder(QuizBuilder qb, int choiceCount, int questionId, int otherQuestionId) : base(
            qb, choiceCount, questionId)
        {
            _mapToQuestionId = otherQuestionId;
        }

        internal override IConstraint<string> BuildConstraint(List<IOrderedVariable> variables)
        {
            var other = variables.First(v => v.Id == _mapToQuestionId);
            return new ChoiceEqualsConstraint(GetMe(variables), other, Choices);
        }

        internal override void Validate()
        {
            if (Builder._nextQuestionId <= _mapToQuestionId)
            {
                throw new Exception($"Question #{_mapToQuestionId} not found");
            }

            var domain = Builder.Domain.Values.ToList();
            var badChoices = Choices.Where(c => !Builder.Domain.Values.Contains(c)).ToList();
            if (badChoices.Count > 0)
            {
                throw new Exception(
                    $"Choices {{{string.Join(",", badChoices)}}} are not in domain {{{string.Join(",", domain)}}}");
            }

            ValidateChoices();
        }
    }

    public class CountOfChoiceQuestionBuilder : QuestionBuilder<int, CountOfChoiceQuestionBuilder>
    {
        private readonly string _choiceToCount;

        internal CountOfChoiceQuestionBuilder(QuizBuilder qb, int choiceCount, int questionId, string choiceToCount) :
            base(qb, choiceCount, questionId)
        {
            _choiceToCount = choiceToCount;
        }

        internal override IConstraint<string> BuildConstraint(List<IOrderedVariable> variables) =>
            new AnswerCountConstraint(GetMe(variables), variables, _choiceToCount, Choices);

        internal override void Validate()
        {
            var domain = Builder.Domain.Values.ToList();
            if (!domain.Contains(_choiceToCount))
            {
                throw new Exception($"Answer to count {_choiceToCount} not in domain {{{string.Join(",", domain)}}}");
            }

            var maxQuestionId = Builder._nextQuestionId - 1;
            var badChoices = Choices.Where(choice => choice > maxQuestionId || choice < 0).ToList();
            if (badChoices.Count > 0)
            {
                throw new Exception(
                    $"Choices {{{string.Join(",", badChoices)}}} must be between 0 and the question count {maxQuestionId}, inclusive");
            }

            ValidateChoices();
        }
    }

    public class FirstWithChoiceQuestionBuilder : QuestionBuilder<int?, FirstWithChoiceQuestionBuilder>
    {
        private readonly string _choiceToCount;
        private readonly bool _isReverse;

        private int? _threshold;
        private bool _isAfter;

        private int MaxQuestionId =>
            _threshold == null || _isAfter ? Builder._nextQuestionId - 1 : _threshold.Value - 1;

        private int MinQuestionId => _threshold == null || !_isAfter ? 1 : _threshold.Value + 1;

        internal FirstWithChoiceQuestionBuilder(QuizBuilder qb, int choiceCount, int questionId, string choiceToCount,
            bool isReverse) : base(qb, choiceCount, questionId)
        {
            _choiceToCount = choiceToCount;
            _isReverse = isReverse;
        }

        public FirstWithChoiceQuestionBuilder Before(int threshold)
        {
            _isAfter = false;
            _threshold = threshold;
            return this;
        }

        public FirstWithChoiceQuestionBuilder After(int threshold)
        {
            _isAfter = true;
            _threshold = threshold;
            return this;
        }

        internal override IConstraint<string> BuildConstraint(List<IOrderedVariable> variables)
        {
            // what's my range?
            var rangeToCheck = new List<IOrderedVariable>();
            for (var i = MinQuestionId; i <= MaxQuestionId; i++)
            {
                rangeToCheck.Add(variables.First(v => v.Id == i));
            }

            return new FirstWithChoiceConstraint(GetMe(variables), rangeToCheck, _choiceToCount, Choices, _isReverse);
        }

        internal override void Validate()
        {
            var domain = Builder.Domain.Values.ToList();
            if (!domain.Contains(_choiceToCount))
            {
                throw new Exception($"Answer to find {_choiceToCount} not in domain {{{string.Join(",", domain)}}}");
            }

            var badChoices = Choices
                .Where(choice => choice != null && (choice > MaxQuestionId || choice < MinQuestionId))
                .Select(v => v!.Value).ToList();
            if (badChoices.Count > 0)
            {
                throw new Exception(
                    $"Choices {{{string.Join(",", badChoices)}}} must be between {MinQuestionId} and {MaxQuestionId}, inclusive");
            }

            ValidateChoices();
        }
    }

    public class MostLeastCommonQuestionBuilder : QuestionBuilder<string?, MostLeastCommonQuestionBuilder>
    {
        private readonly bool _isLeast;

        internal MostLeastCommonQuestionBuilder(QuizBuilder qb, int choiceCount, int questionId, bool isLeast) :
            base(qb, choiceCount, questionId)
        {
            _isLeast = isLeast;
        }

        internal override IConstraint<string> BuildConstraint(List<IOrderedVariable> variables) =>
            new MostLeastCommonConstraint(GetMe(variables), variables, Choices, _isLeast);

        internal override void Validate()
        {
            var domain = Builder.Domain.Values.ToList();
            var badChoices = Choices.Where(c => c != null && !domain.Contains(c)).ToList();
            if (badChoices.Count > 0)
            {
                throw new Exception(
                    $"Choices {{{string.Join(",", badChoices)}}} are not in domain {{{string.Join(",", domain)}}}");
            }

            ValidateChoices();
        }
    }

    public class OnlyConsecutiveSameQuestionBuilder : QuestionBuilder<int, OnlyConsecutiveSameQuestionBuilder>
    {
        private readonly int _consecutiveSameSize;

        internal OnlyConsecutiveSameQuestionBuilder(QuizBuilder qb, int choiceCount, int questionId,
            int consecutiveSame) : base(qb, choiceCount, questionId)
        {
            _consecutiveSameSize = consecutiveSame;
        }

        internal override IConstraint<string> BuildConstraint(List<IOrderedVariable> variables)
        {
            // need to convert "starting ids" to the actual window
            var windows = Choices.Select(c =>
            {
                var window = new List<int>();
                for (var i = 0; i < _consecutiveSameSize; i++)
                {
                    window.Add(c + i);
                }

                return window;
            }).ToList();
            return new OnlyConsecutiveSameConstraint(GetMe(variables), variables, windows);
        }

        internal override void Validate()
        {
            // if 8 questions and our set size is 3, the highest one can be 6 -> checking 6-8
            // if 8 questions, the next id at this point is 9, so 9-3 = 6.
            var maxQuestionId = Builder._nextQuestionId - _consecutiveSameSize;
            var badChoices = Choices.Where(c => c < 1 || c > maxQuestionId).ToList();
            if (badChoices.Count > 0)
            {
                throw new Exception(
                    $"Choices {{{string.Join(",", badChoices)}}} must be between 1 and {maxQuestionId}, inclusive");
            }

            ValidateChoices();
        }
    }

    public class OnlySameChoiceQuestionBuilder : QuestionBuilder<int, OnlySameChoiceQuestionBuilder>
    {
        internal OnlySameChoiceQuestionBuilder(QuizBuilder qb, int choiceCount, int questionId) : base(qb, choiceCount,
            questionId)
        {
        }

        internal override IConstraint<string> BuildConstraint(List<IOrderedVariable> variables) =>
            new OnlySameChoiceConstraint(GetMe(variables), variables, Choices);

        internal override void Validate()
        {
            var badChoices = Choices.Where(c => c < 1 || c >= Builder._nextQuestionId).ToList();
            if (badChoices.Count > 0)
            {
                throw new Exception(
                    $"Choices {{{string.Join(",", badChoices)}}} must be between 1 and {Builder._nextQuestionId - 1}, inclusive");
            }

            ValidateChoices();
        }
    }
}