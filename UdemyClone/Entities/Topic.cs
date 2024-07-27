namespace UdemyClone.Entities
{
    public class Topic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid SubCategoryId { get; set; }
        public SubCategory SubCategory { get; set; }
    }
}
