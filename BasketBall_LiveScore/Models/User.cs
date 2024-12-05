using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BasketBall_LiveScore.Models;
public class User
{

    public enum Role
    {
        None,
        Encoder,
        Admin
    }

    [Required]
    public ulong Id { get; set; }
    [MaxLength(255)]
    [Required]
    public string Username { get; set; } = string.Empty;
    [MaxLength(50)]
    [Required]
    public string Password { get; set; } = string.Empty;
    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Required]
    public Role Permission { get; set; } = Role.None;
}
