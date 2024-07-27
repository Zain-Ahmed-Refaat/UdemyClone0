namespace UdemyClone.Services
{
    public class UserManagerResponse
    {
        public string Message { get; set; }

        public bool isAuthenticated { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public Guid UserId { get; set; }

        public List<string> Roles { get; set; }

        public string Token { get; set; }

        public DateTime ExpirationDate { get; set; }
    }
}
