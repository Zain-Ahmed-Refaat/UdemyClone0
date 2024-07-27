namespace UdemyClone.Entities
{
    public class UserWithRolesDTO<Guid>
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
    }
}
