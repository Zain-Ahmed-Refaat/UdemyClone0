using System.ComponentModel.DataAnnotations;

namespace UdemyClone.Models
{
    public class RegisterationModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 8)]
        public string Password { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 8)]
        public string ConfirmPassword { get; set; }

        [Required]
        public UserRole? Role { get; set; } = UserRole.User;
    }

    public enum UserRole
    {
        User = 1,
        Admin,
        Instructor,
        Student
    }

}
