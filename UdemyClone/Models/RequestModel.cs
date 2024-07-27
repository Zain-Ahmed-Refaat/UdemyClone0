using System.ComponentModel.DataAnnotations;

namespace UdemyClone.Models
{
    public class CreateQuizRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid LessonId { get; set; }
        public List<CreateQuestionRequest> Questions { get; set; }
    }

    public class CreateQuestionRequest
    {
        public string Text { get; set; }
        public Guid CorrectAnswerId { get; set; }
        public List<CreateAnswerRequest> Answers { get; set; }
    }

    public class CreateAnswerRequest
    {
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
    }

    public class SubmitQuizRequest
    {
        [Required]
        public List<StudentAnswerRequest> Answers { get; set; }
    }

    public class StudentAnswerRequest
    {
        [Required]
        public Guid QuestionId { get; set; }
        [Required]
        public Guid AnswerId { get; set; }
    }


}
