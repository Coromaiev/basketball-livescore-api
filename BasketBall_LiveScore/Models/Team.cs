using System.ComponentModel.DataAnnotations;

namespace BasketBall_LiveScore.Models;
public class Team
{
    public ulong Id { get; set; }
    [MaxLength(255)]
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public List<Player> Players { get; set; } = new();
}
