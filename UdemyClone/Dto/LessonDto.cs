namespace UdemyClone.Dto
{
    public class LessonDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid LessonId { get; set; }
        public Guid CourseId { get; set; }
    }
}
