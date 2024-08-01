using System.ComponentModel.DataAnnotations;

namespace UdemyClone.Models
{
    public class CourseModel
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        public Guid TopicId { get; set; }
    }
}
