using System.ComponentModel.DataAnnotations;

namespace BasketBall_LiveScore;
public class Team
{
    public int Id { get; set; }
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [Required]
    public List<Player> Players { get; set; } = new();
}
