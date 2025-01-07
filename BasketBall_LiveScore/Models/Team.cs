using System.ComponentModel.DataAnnotations;

namespace BasketBall_LiveScore.Models;
public class Team
{
    [Required]
    public Guid Id { get; set; } = Guid.NewGuid();
    [MaxLength(255)]
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public List<Player> Players { get; set; } = new();
}

public record TeamDto(Guid Id, string Name, List<PlayerDto> Players);
public record TeamAddPlayerDto(Guid NewPlayerId, byte NewPlayerNumber);
