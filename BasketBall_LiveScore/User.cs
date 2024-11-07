using System.ComponentModel.DataAnnotations;

namespace BasketBall_LiveScore;
public class User
{
    public int ID { get; set; }
    [MaxLength(250)]
    public string Username { get; set; } = string.Empty;
    [Required]
    [MaxLength(50)]
    public string Password { get; set; } = string.Empty;
    [Required]
    public string Email { get; set; } = string.Empty;
}
