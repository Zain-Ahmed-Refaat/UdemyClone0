namespace UdemyClone.Dto
{
    public class CreateTopicDto
    {
        public string Name { get; set; }
        public Guid SubCategoryId { get; set; }
    }

    public class UpdateTopicDto
    {
        public string NewName { get; set; }
    }
}
