using BasketBall_LiveScore.Exceptions;
using BasketBall_LiveScore.Models;
using BasketBall_LiveScore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BasketBall_LiveScore.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly IUserService UserService;
        private readonly ILogger<UserController> Logger;

        public UserController(IConfiguration configuration, IUserService userService, ILogger<UserController> logger)
        {
            Configuration = configuration;
            UserService = userService;
            Logger = logger;
        }

        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserLoginDto login)
        {
            try
            {
                var loggedInUser = await UserService.GetByEmailAndPassword(login);
                var tokenString = GenerateToken(loggedInUser);
                return Ok(new { token = tokenString, user = loggedInUser});
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error during login");
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserCreateDto user)
        {
            try
            {
                var newUser = await UserService.Create(user);
                var tokenString = GenerateToken(newUser);
                return CreatedAtAction(nameof(Get), new { id = newUser.Id }, new { token = tokenString, user = newUser});
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error while registering");
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Authorize(Policy = "AuthenticationRequired")]
        [Route("{id:guid}")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UserUpdateDto user, [FromRoute] Guid id)
        {
            try
            {
                Logger.LogDebug(user.ToString());
                var updatedUser = await UserService.Update(id, user);
                return NoContent();

            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error while performing update");
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [Route("{id:guid}")]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            try
            {
                await UserService.Delete(id);
                return NoContent();
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error while deleting");
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [Route("{id:guid}")]
        [HttpGet]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            try
            {
                var user = await UserService.GetById(id);
                return Ok(user);
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error while fetching");
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Authorize(Policy = "EncoderAccess")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] Role? role = null)
        {
            try
            {
                var usersList =  new List<UserDto>();
                var users = role.HasValue ? UserService.GetByRole(role.Value) : UserService.GetAll();
                await foreach (var user in users)
                {
                    usersList.Add(user);
                }
                return Ok(usersList);
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error while fetching");
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        private string GenerateToken(UserDto user)
        {
            var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Permission.ToString())
                };

            var token = new JwtSecurityToken
                (
                    issuer: Configuration["JWT:Issuer"],
                    audience: Configuration["JWT:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(int.Parse(Configuration["JWT:LifetimeDurationMinutes"])),
                    notBefore: DateTime.Now,
                    signingCredentials: new SigningCredentials
                        (
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Key"])),
                            SecurityAlgorithms.HmacSha256
                        )
                );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenString;
        }
    }
}
