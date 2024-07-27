namespace UdemyClone.Dto
{
    public class QuizResultDto
    {
        public decimal Score { get; set; }
        public bool Passed { get; set; }
        public DateTime DateTaken { get; set; }
        public Guid StudentId { get; set; }
        public string StudentName { get; set; }
        public Guid QuizId { get; set; }
        public string QuizTitle { get; set; }

    }
}
