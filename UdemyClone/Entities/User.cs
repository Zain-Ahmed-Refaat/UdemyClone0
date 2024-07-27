using Microsoft.AspNetCore.Identity;

namespace UdemyClone.Entities
{
    public class User : IdentityUser<Guid>
    {
        public ICollection<StudentCourse> Enrollments { get; set; }

    }
}
