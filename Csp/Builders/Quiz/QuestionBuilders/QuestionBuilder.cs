using Csp.Interfaces;

namespace Csp.Builders.Quiz;

public partial class QuizBuilder
{
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
}