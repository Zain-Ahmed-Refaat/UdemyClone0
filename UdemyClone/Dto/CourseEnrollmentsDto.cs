namespace UdemyClone.Dto
{
    public class CourseEnrollmentsDto
    {
        public Guid CourseId { get; set; }
        public string CourseName { get; set; }
        public List<StudentDto> Enrollments { get; set; } = new List<StudentDto>();
    }

}
