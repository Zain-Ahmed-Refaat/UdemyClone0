using System.ComponentModel.DataAnnotations;

namespace UdemyClone.Dto
{
    public class CreateTopicDto
    {
        public string Name { get; set; }
        public Guid SubCategoryId { get; set; }
    }

    public class UpdateTopicDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Topic name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Topic name must be between 3 and 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "SubCategoryId is required.")]
        public Guid SubCategoryId { get; set; }
    }
}
