namespace UdemyClone.Entities
{
    public class StudentQuiz
    {
        public Guid Id { get; set; }

        public Guid StudentId { get; set; }
        public User Student { get; set; }

        public Guid QuizId { get; set; }
        public Quiz Quiz { get; set; }

        public DateTime DateTaken { get; set; }
        public int Score { get; set; }
        public bool Passed { get; set; }

        public ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();
    }
}
