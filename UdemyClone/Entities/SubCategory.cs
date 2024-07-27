namespace UdemyClone.Entities
{
    public class SubCategory
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
