using System.ComponentModel.DataAnnotations;

namespace BasketBall_LiveScore.Models;
public class Match
{
    [Required]
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    public byte QuarterDuration { get; set; }
    [Required]
    public byte NumberOfQuarters { get; set; }
    [Required]
    public byte TimeOutDuration { get; set; }
    [Required]
    public Team Visitors { get; set; }
    [Required]
    public Team Hosts { get; set; }
    [Required]
    public Guid VisitorsId { get; set; }
    [Required]
    public Guid HostsId { get; set; }
    public User? PrepEncoder { get; set; }
    public User? PlayEncoder { get; set; }
    public Guid? PrepEncoderId { get; set; }
    public Guid? PlayEncoderId { get; set; }
    [Required]
    public ulong HostsScore { get; set; } = 0;
    [Required]
    public ulong VisitorsScore { get; set; } = 0;
    [Required]
    public List<MatchEvent> Events { get; set; } = [];
    [MaxLength(5)]
    public List<Player>? VisitorsStartingPlayers = new(5); 
    [MaxLength(5)]
    public List<Player>? HostsStartingPlayers = new(5);
}

// MISSES EVENTS AND STARTING PLAYERS. TEAMS SHOULD HAVE THEIR IDS REPLACED WITH DTOS
public record MatchDto(Guid Id, byte QuarterDuration, byte QuarterNumbers, byte TimeoutDuration, Guid VisitorsId, Guid HostsId, Guid PrepEncoderId, Guid PlayEncoderId, ulong HostsScore, ulong VisitorsScore);