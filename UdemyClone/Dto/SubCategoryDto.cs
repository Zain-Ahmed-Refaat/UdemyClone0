using System.ComponentModel.DataAnnotations;

namespace UdemyClone.Dto
{
    public class CreateSubCategoryDto
    {
        public string Name { get; set; }
        public Guid CategoryId { get; set; }

    }
    public class SubCategoryUpdateDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "SubCategory name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "SubCategory name must be between 3 and 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "CategoryId is required.")]
        public Guid CategoryId { get; set; }
    }
}
