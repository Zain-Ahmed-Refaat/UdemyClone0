namespace UdemyClone.Entities
{
    public class Quiz
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid LessonId { get; set; }
        public Lesson Lesson { get; set; }
        public ICollection<Question> Questions { get; set; }
    }
}
