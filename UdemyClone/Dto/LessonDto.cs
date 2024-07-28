namespace UdemyClone.Dto
{
    public class LessonDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public Guid CourseId { get; set; }
    }
}
