namespace UdemyClone.Dto
{
    public class QuizResultDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; }
        public decimal Score { get; set; }
        public bool Passed { get; set; }
        public DateTime DateTaken { get; set; }

    }
}
