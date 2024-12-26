using System.ComponentModel.DataAnnotations;

namespace BasketBall_LiveScore.Models;
public class Match
{
    [Required]
    public ulong Id { get; set; }
    [Required]
    public byte QuarterDuration { get; set; }
    [Required]
    public byte NumberOfQuarters { get; set; }
    [Required]
    public byte TimeOutDuration { get; set; }
    [Required]
    public Team? Visitors { get; set; }
    [Required]
    public Team? Hosts { get; set; }
    [Required]
    public ulong? VisitorsId { get; set; }
    [Required]
    public ulong? HostsId { get; set; }
    [Required]
    public User? PrepEncoder { get; set; }
    [Required]
    public User? PlayEncoder { get; set; }
    [Required]
    public ulong? PrepEncoderId { get; set; }
    [Required]
    public ulong? PlayEncoderId { get; set; }
    [Required]
    public ulong HostsScore { get; set; } = 0;
    [Required]
    public ulong? VisitorsScore { get; set; } = 0;
    [Required]
    public List<MatchEvent> Events { get; set; } = new();
    [Required]
    [MaxLength(5)]
    public List<Player> VisitorsStartingPlayers = new(5); 
    [Required]
    [MaxLength(5)]
    public List<Player> HostsStartingPlayers = new(5);
}
