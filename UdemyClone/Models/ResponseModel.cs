namespace UdemyClone.Models
{
    public class QuizDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<QuestionDto> Questions { get; set; }
    }

    public class QuestionDto
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public List<AnswerDto> Answers { get; set; }
    }

    public class AnswerDto
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
    }

}
