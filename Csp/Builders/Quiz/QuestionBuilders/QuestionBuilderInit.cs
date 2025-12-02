namespace Csp.Builders.Quiz;

public partial class QuizBuilder
{
    public class QuestionBuilderInit(QuizBuilder qb, int choiceCount, int questionId)
    {
        private void Add(QuestionBuilder questionBuilder)
        {
            qb._questions.Add(questionBuilder);
        }

        // ==== DIRECT MAP QUESTIONS ====
        public MapChoiceQuestionBuilder AnswerToQuestion(int otherQuestionId)
        {
            var b = new MapChoiceQuestionBuilder(qb, choiceCount, questionId, otherQuestionId);
            Add(b);
            return b;
        }
        
        // ==== COUNT OF ANSWER QUESTIONS ====
        public CountOfChoiceQuestionBuilder CountOfAnswer(string answer) => CountOfChoices([answer]);

        public CountOfChoiceQuestionBuilder CountOfVowels() => CountOfChoices(["A", "E"]);

        public CountOfChoiceQuestionBuilder CountOfConsonants() => CountOfChoices(["B", "C", "D"]);

        private CountOfChoiceQuestionBuilder CountOfChoices(List<string> choices)
        {
            var b = new CountOfChoiceQuestionBuilder(qb, choiceCount, questionId, choices);
            Add(b);
            return b;
        }
        
        // ==== FIRST QUESTION WITH ANSWER QUESTIONS ====
        public FirstWithChoiceQuestionBuilder First() => FirstWithChoice(false, false);

        public FirstWithChoiceQuestionBuilder Last() => FirstWithChoice(true, false);
        
        public FirstWithChoiceQuestionBuilder Next() => FirstWithChoice(false, true);
        
        public FirstWithChoiceQuestionBuilder Previous() => FirstWithChoice(true, true);

        private FirstWithChoiceQuestionBuilder FirstWithChoice(bool isReverse, bool isNext)
        {
            var b = new FirstWithChoiceQuestionBuilder(qb, choiceCount, questionId, isReverse, isNext);
            Add(b);
            return b;
        }

        // ==== MOST/LEAST COMMON/COUNT OF MOST COMMON QUESTIONS ====
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

        public MostCommonCountQuestionBuilder CountOfMostCommonAnswer()
        {
            var b = new MostCommonCountQuestionBuilder(qb, choiceCount, questionId);
            Add(b);
            return b;
        }

        // ==== ONLY CONSECUTIVE N QUESTIONS ====
        public OnlyConsecutiveSameQuestionBuilder OnlyConsecutiveSameSetOf(int n)
        {
            var b = new OnlyConsecutiveSameQuestionBuilder(qb, choiceCount, questionId, n);
            Add(b);
            return b;
        }

        // ==== ONLY QUESTION WITH SAME ANSWER QUESTIONS ==== 
        public OnlySameChoiceQuestionBuilder OnlyQuestionWithTheSameAnswer()
        {
            var b = new OnlySameChoiceQuestionBuilder(qb, choiceCount, questionId);
            Add(b);
            return b;
        }
    }
}