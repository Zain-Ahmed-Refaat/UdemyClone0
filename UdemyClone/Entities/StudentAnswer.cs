namespace UdemyClone.Entities
{
    public class StudentAnswer
    {
        public Guid Id { get; set; }

        public Guid StudentQuizId { get; set; }
        public StudentQuiz StudentQuiz { get; set; }

        public Guid QuestionId { get; set; }
        public Question Question { get; set; }

        public Guid AnswerId { get; set; }
        public Answer Answer { get; set; }
    }

}
