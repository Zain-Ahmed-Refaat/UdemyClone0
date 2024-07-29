using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using UdemyClone.Data;
using UdemyClone.Entities;
using UdemyClone.Models;
using UdemyClone.Services.IServices;


namespace UdemyClone.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> userManager;
        private readonly IConfiguration configuration;
        private readonly RoleManager<IdentityRole<Guid>> roleManager;
        private readonly ApplicationDbContext context;

        public UserService(UserManager<User> userManager, IConfiguration configuration, RoleManager<IdentityRole<Guid>> roleManager, ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.roleManager = roleManager;
            this.context = context;
        }

        public async Task<UserManagerResponse> RegisterUserAsync(RegisterationModel model)
        {

            var userEmail = await userManager.FindByEmailAsync(model.Email);
            var userName = await userManager.FindByNameAsync(model.Username);


            switch (userEmail, userName)
            {
                case (not null, _):
                    return new UserManagerResponse { Message = $"{model.Email} is already registered!" };

                case (_, not null):
                    return new UserManagerResponse { Message = $"Username is already taken. Please try another name." };
            }

            switch (model)
            {
                case null:
                    throw new ArgumentNullException(nameof(model), "Please provide valid data.");

                case { Email: null or "" }:
                    throw new ArgumentNullException(nameof(model.Email), "Email Field Is Required");

                case { Email: var email } when !IsValidEmail(email):
                    return new UserManagerResponse { Message = "Please provide a valid email address." };

                case { Username: null or "" }:
                    throw new ArgumentNullException(nameof(model.Username), "Username Field Is Required");

                case { Password: null }:
                    throw new ArgumentNullException(nameof(model.Password), "Password Field Is Required");

                case { ConfirmPassword: null }:
                    throw new ArgumentNullException(nameof(model.ConfirmPassword), "Confirm Password Field Is Required");

                case { Password: var pwd, ConfirmPassword: var confirmPwd } when pwd != confirmPwd:
                    return new UserManagerResponse { Message = "The password and confirmation password do not match." };

                case { Role: UserRole.Admin }:
                    return new UserManagerResponse {
                        Message = "You Cannot Register as an Admin.",
                        isAuthenticated = false
                    };

                default:
                    break;
            }

            var user = new User
            {
                UserName = model.Username,
                Email = model.Email
            };

            var result = await userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var roleName = model.Role.ToString();
                var roleResult = await userManager.AddToRoleAsync(user, roleName);

                if (!roleResult.Succeeded)
                {
                    return new UserManagerResponse
                    {
                        isAuthenticated = false,
                        Message = "Role assignment failed."
                    };
                }

                await context.SaveChangesAsync();

                var JwtSecurityToken = await GenerateJwtToken(user);

                return new UserManagerResponse
                {
                    Message = "User registered successfully!",
                    isAuthenticated = true,
                    Roles = new List<string> { model.Role.ToString() },
                    Email = model.Email,
                    Username = model.Username,
                    UserId = user.Id,
                    ExpirationDate = JwtSecurityToken.ValidTo,
                    Token = new JwtSecurityTokenHandler().WriteToken(JwtSecurityToken)
                };
            }
            else
            {
                var errors = string.Join(",", result.Errors.Select(e => e.Description));
                return new UserManagerResponse { Message = $"Failed to register: {errors}" };
            }
        }

        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }


        private async Task<JwtSecurityToken> GenerateJwtToken(User user)
             {
                 var userClaims = await userManager.GetClaimsAsync(user);
                 var roles = await userManager.GetRolesAsync(user);
                 var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

                 var key = configuration["JWT:Key"];
                 var issuer = configuration["JWT:Issuer"];
                 var audience = configuration["JWT:Audience"];

                 var claims = new List<Claim>
                     {
                         new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                         new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                         new Claim(JwtRegisteredClaimNames.Email, user.Email),
                         new Claim("UserID", user.Id.ToString()),
                         new Claim(ClaimTypes.Name, user.UserName)
                     };

                 claims.AddRange(userClaims);
                 claims.AddRange(roleClaims);

                 var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                 var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

                 var jwtSecurityToken = new JwtSecurityToken(
                     issuer: issuer,
                     audience: audience,
                     claims: claims,
                     expires: DateTime.UtcNow.AddDays(30),
                     signingCredentials: signingCredentials);

                 return jwtSecurityToken;
             }



        public async Task<UserManagerResponse> LoginUserAsync(LoginModel model)
        {
            var userResponse = new UserManagerResponse();
            var response = model switch
            {
                null => new UserManagerResponse
                {
                    Message = "Login model cannot be null.",
                    isAuthenticated = false
                },

                { Email: null or "" } => new UserManagerResponse
                {
                    Message = "Email cannot be empty.",
                    isAuthenticated = false
                },

                { Email: var email } when !IsValidEmail(email) => new UserManagerResponse
                {
                    Message = "Please provide a valid email address.",
                    isAuthenticated = false
                },

                { Password: null or "" } => new UserManagerResponse
                {
                    Message = "Password cannot be empty.",
                    isAuthenticated = false
                },

                _ => null
            };

            if (response != null)
            {
                return response;
            }

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new UserManagerResponse
                {
                    Message = "Invalid email or password.",
                    isAuthenticated = false
                };
            }

            var isPasswordValid = await userManager.CheckPasswordAsync(user, model.Password);
            if (!isPasswordValid)
            {
                return new UserManagerResponse
                {
                    Message = "Invalid email or password.",
                    isAuthenticated = false
                };
            }

            var JwtSecurityToken = await GenerateJwtToken(user);
            var rolesList = await userManager.GetRolesAsync(user);

            userResponse.Username = user.UserName;
            userResponse.Email = user.Email;
            userResponse.isAuthenticated = true;
            userResponse.ExpirationDate = JwtSecurityToken.ValidTo;
            userResponse.Token = new JwtSecurityTokenHandler().WriteToken(JwtSecurityToken);
            userResponse.Roles = rolesList.ToList();

            return userResponse;
        }



        public async Task<string> AddRoleAsync(AddRoleModel model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);

            if (user is null || !await roleManager.RoleExistsAsync(model.Role))
                return "Invalid user ID or Role";

            if (await userManager.IsInRoleAsync(user, model.Role))
                return "User already assigned to this role";

            var result = await userManager.AddToRoleAsync(user, model.Role);

            return result.Succeeded ? string.Empty : "Sonething went wrong";
        }

        public async Task<IEnumerable<UserWithRolesDTO<Guid>>> GetAllUsersAsync()
        {
            return await context.Users
                .Join(context.UserRoles,
                      user => user.Id,
                      userRole => userRole.UserId,
                      (user, userRole) => new { user, userRole })
                .Join(context.Roles,
                      ur => ur.userRole.RoleId,
                      role => role.Id,
                      (ur, role) => new
                      {
                          ur.user.Id,
                          ur.user.UserName,
                          ur.user.Email,
                          RoleName = role.Name
                      })
                .GroupBy(x => new { x.Id, x.UserName, x.Email })
                .Select(g => new UserWithRolesDTO<Guid>
                {
                    UserId = g.Key.Id,
                    UserName = g.Key.UserName,
                    Email = g.Key.Email,
                    Roles = g.Select(x => x.RoleName).ToList()
                })
                .ToListAsync();
        }

    }
}
