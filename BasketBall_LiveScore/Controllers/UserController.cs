using BasketBall_LiveScore.Models;
using BasketBall_LiveScore.Services;
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

        public UserController(IConfiguration configuration, IUserService userService)
        {
            Configuration = configuration;
            UserService = userService;
        }

        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserLoginDto login)
        {
            try
            {
                if (string.IsNullOrEmpty(login.Email) || string.IsNullOrEmpty(login.Password)) return BadRequest("Email and/or password is missing");
                var loggedInUser = await UserService.GetByEmailAndPassword(login);
                if (loggedInUser is null)
                {
                    return Unauthorized("Email or password is invalid");
                }
                var tokenString = GenerateToken(loggedInUser);
                return Ok(new { token = tokenString, user = loggedInUser});
            }
            catch (Exception ex)
            {
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserCreateDto user)
        {
            try
            {
                if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
                {
                    return BadRequest("Email, username and/or password is invalid");
                }
                var newUser = await UserService.Create(user);
                if (newUser is null)
                {
                    return Conflict($"{user.Email} is already in use");
                }
                var tokenString = GenerateToken(newUser);
                return CreatedAtAction(nameof(Get), new { id = newUser.Id }, new { token = tokenString, user = newUser});
            }
            catch (Exception ex)
            {
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("{id:guid}")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UserUpdateDto user, [FromRoute] Guid id)
        {
            try
            {
                if ((!string.IsNullOrEmpty(user.NewPassword) || !string.IsNullOrEmpty(user.NewEmail)) && string.IsNullOrEmpty(user.CurrentPassword))
                    return BadRequest("Current password is required to update email or password");
                var updatedUser = await UserService.Update(id, user);
                if (updatedUser is null)
                {
                    return NotFound($"No user found with id {id}");
                }
                return NoContent();

            } catch (Exception ex)
            {
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }

        }

        [Route("{id:guid}")]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            try
            {
                var user = await UserService.GetById(id);
                if (user is null) return NotFound($"No user found with id {id}");
                await UserService.Delete(id);
                return NoContent();
            } catch (Exception ex)
            {
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("{id:guid}")]
        [HttpGet]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            try
            {
                var user = await UserService.GetById(id);
                if (user is null) return NotFound($"User with id {id} not found");
                return Ok(user);
            } catch (Exception ex)
            {
                
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, "An unexpected error occurred");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] Role? role = null)
        {
            try
            {
                var users = role.HasValue ? await Task.Run(() => UserService.GetByRole((Role)role)) : await Task.Run(() => UserService.GetAll());
                if (users is null || !users.Any())
                {
                    var message = "No users found" + (role is not null ? $" with role {role}" : "");
                    return NotFound(message);
                }
                return Ok(users);
            } catch (Exception ex)
            {
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
