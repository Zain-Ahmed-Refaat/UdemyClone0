using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UdemyClone.Models;
using UdemyClone.Services.IServices;

namespace UdemyClone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserService userService;


        public AuthenticationController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterationModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await userService.RegisterUserAsync(model);

            if (!result.isAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await userService.LoginUserAsync(model);

            if (!result.isAuthenticated)
                return BadRequest(result.Message);

            return Ok(new { result.Roles, result.Token, result.ExpirationDate });
        }

        [HttpGet("Get-Roles")]
        [Authorize(Roles = "Admin")]
        public ActionResult<IEnumerable<string>> GetRoles()
        {
            var roles = Enum.GetValues(typeof(UserRole))
                .Cast<UserRole>()
                .Select(role => $"{role} {(int)role}")
                .ToList();

            return Ok(roles);
        }

        [HttpPost("Add-Role-To-User")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddRoleAsync([FromBody] AddRoleModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await userService.AddRoleAsync(model);

            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);

            return Ok(model);
        }

        [HttpGet("Get-All-Users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var usersWithRoles = await userService.GetAllUsersAsync();
            return usersWithRoles.Any() ? Ok(usersWithRoles) : NotFound("No users found.");
        }


        [HttpGet("get-id-from-token")]
        public IActionResult GetIdFromTokenAction()
        {
            try
            {
                var userId = GetIdFromToken();
                return Ok(new { UserId = userId });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
        }

        [HttpGet("get-all-claims")]
        public IActionResult GetAllClaims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Ok(claims);
        }

        private Guid GetIdFromToken()
        {
            var userIdClaim = User.FindFirstValue("UserID");

            var userID = string.IsNullOrEmpty(userIdClaim) ?
                         throw new UnauthorizedAccessException("User ID claim not found in the token.") :
                          Guid.Parse(userIdClaim);

            return userID;
        }

    }
}
