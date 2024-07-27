namespace UdemyClone.Entities
{
    public class StudentLessonProgress
    {
        public Guid Id { get; set; }

        public Guid StudentId { get; set; }
        public User Student { get; set; }

        public Guid LessonId { get; set; }
        public Lesson Lesson { get; set; }

        public bool IsWatched { get; set; }

    }
}
