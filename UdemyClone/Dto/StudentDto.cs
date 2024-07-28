using UdemyClone.Entities;

namespace UdemyClone.Dto
{
    public class StudentDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<EnrollmentDto> Enrollments { get; set; } = new List<EnrollmentDto>();
    }

    public class EnrollmentDto
    {
        public Guid CourseId { get; set; }
        public string CourseName { get; set; }
    }

}
