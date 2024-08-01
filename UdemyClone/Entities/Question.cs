namespace UdemyClone.Entities
{
    public class Question
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public Guid QuizId { get; set; }
        public Quiz Quiz { get; set; }
        public ICollection<Answer> Answers { get; set; }
        public Guid CorrectAnswerId { get; set; }
        public Answer CorrectAnswer { get; set; }
    }
}