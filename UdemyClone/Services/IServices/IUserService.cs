using UdemyClone.Entities;
using UdemyClone.Models;

namespace UdemyClone.Services.IServices
{
    public interface IUserService
    {
        Task<UserManagerResponse> RegisterUserAsync(RegisterationModel model);
        Task<IEnumerable<UserWithRolesDTO<Guid>>> GetAllUsersAsync();
        Task<UserManagerResponse> LoginUserAsync(LoginModel model);
        Task<string> AddRoleAsync(AddRoleModel model);
    }
}
