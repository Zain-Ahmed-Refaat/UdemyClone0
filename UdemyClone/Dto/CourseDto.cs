namespace UdemyClone.Dto
{
    public class CourseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Topic { get; set; }
        public Guid InstructorId { get; set; }

    }
}
