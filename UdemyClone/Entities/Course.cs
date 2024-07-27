namespace UdemyClone.Entities
{
    public class Course
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid TopicId { get; set; }
        public Topic Topic { get; set; }
        public Guid InstructorId { get; set; }
        public User Instructor { get; set; }

        public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
    }
}
