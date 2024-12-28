using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BasketBall_LiveScore.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Role
{
    None,
    Encoder,
    Admin
}

public class User
{

    [Required]
    public Guid Id { get; set; } = Guid.NewGuid();
    [MaxLength(255)]
    [Required]
    public string Username { get; set; } = string.Empty;
    [MaxLength(50)]
    [Required]
    public string Password { get; set; } = string.Empty;
    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    [Required]
    public Role Permission { get; set; } = Role.None;
}

public record UserLoginDto(string Email, string Password);
public record UserCreateDto(string Email, string Password, string Username, Role Permission);
public record UserUpdateDto(string? NewUsername, string? NewPassword, string? NewEmail, string? CurrentPassword, Role? NewPermission);
public record UserDto(Guid Id, string Username, string Email, Role Permission);
