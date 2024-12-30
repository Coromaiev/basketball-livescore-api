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
    public Team? Winner { get; set; }
    [Required]
    public Guid VisitorsId { get; set; }
    [Required]
    public Guid HostsId { get; set; }
    public Guid? WinnerId { get; set; }
    public User? PrepEncoder { get; set; }
    public Guid? PrepEncoderId { get; set; }
    [Required]
    public List<User> PlayEncoders { get; set; } = [];
    [Required]
    public bool HasStarted { get; set; } = false;
    [Required]
    public bool IsFinished { get; set; } = false;
    [Required]
    public ulong HostsScore { get; set; } = 0;
    [Required]
    public ulong VisitorsScore { get; set; } = 0;
    [Required]
    public List<MatchEvent> Events { get; set; } = [];
    [Required]
    [MaxLength(5)]
    public List<Player> VisitorsStartingPlayers = new(5); 
    [Required]
    [MaxLength(5)]
    public List<Player> HostsStartingPlayers = new(5);
}

// MISSES EVENTS AND STARTING PLAYERS. TEAMS SHOULD HAVE THEIR IDS REPLACED WITH DTOS
public record MatchDto(Guid Id, byte QuarterDuration, byte QuarterNumbers, byte TimeoutDuration, Guid VisitorsId, Guid HostsId, Guid PrepEncoderId, List<Guid> PlayEncoders, ulong HostsScore, ulong VisitorsScore);