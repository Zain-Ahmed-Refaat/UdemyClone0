namespace UdemyClone.Entities
{
    public class StudentCourse
    {
        public Guid StudentId { get; set; }
        public User Student { get; set; }

        public Guid CourseId { get; set; }
        public Course Course { get; set; }
    }
}

