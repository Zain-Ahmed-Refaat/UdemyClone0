namespace UdemyClone.Dto
{
    public class CreateSubCategoryDto
    {
        public string Name { get; set; }
        public Guid CategoryId { get; set; }

    }
    public class UpdateSubCategoryDto
    {
        public string NewName { get; set; }
    }
}
