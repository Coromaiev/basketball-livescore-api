using System.ComponentModel.DataAnnotations;

namespace BasketBall_LiveScore.Models;
public class Player
{
    public ulong Id { get; set; }
    [MaxLength(70)]
    [Required]
    public string FirstName { get; set; } = string.Empty;
    [MaxLength(100)]
    [Required]
    public string LastName { get; set; } = string.Empty;
    [Required]
    public ulong TeamId { get; set; }
    [Required]
    public byte Number { get; set; } = byte.MinValue;
    [Required]
    public Team? Team { get; set; } = null;

}
