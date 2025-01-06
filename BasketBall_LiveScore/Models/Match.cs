using System.ComponentModel.DataAnnotations;

namespace BasketBall_LiveScore.Models;
public class Match
{
    public const int MaxPlayersPerTeam = 5;
    public const int MinScore = 0;
    public const byte StandardQuarterDuration = 10;
    public const byte StandardNumberOfQuarters = 4;
    public const byte StandardTimeOutDurationMins = 1;

    [Required]
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    public byte QuarterDuration { get; set; } = StandardQuarterDuration;
    [Required]
    public byte NumberOfQuarters { get; set; } = StandardNumberOfQuarters;
    [Required]
    public byte TimeOutDuration { get; set; } = StandardTimeOutDurationMins;
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
    public ulong HostsScore { get; set; } = MinScore;
    [Required]
    public ulong VisitorsScore { get; set; } = MinScore;
    [Required]
    public List<MatchEvent> Events { get; set; } = [];
    [Required]
    [MaxLength(MaxPlayersPerTeam)]
    public List<Player> VisitorsStartingPlayers = new(MaxPlayersPerTeam); 
    [Required]
    [MaxLength(MaxPlayersPerTeam)]
    public List<Player> HostsStartingPlayers = new(MaxPlayersPerTeam);
}
public record MatchDto(Guid Id, byte QuarterDuration, byte QuarterNumber, byte TimeoutDuration, TeamDto Visitors, TeamDto Hosts, List<Guid> FieldPlayers, UserDto? PrepEncoder, List<UserDto> PlayEncoders, ulong HostsScore, ulong VisitorsScore , (List<FaultDto> Faults, List<PlayerChangeDto> PlayerChanges, List<ScoreChangeDto> ScoreChanges, List<TimeOutDto> TimeOuts) Events);
public record MatchCreateDto(byte? QuarterDuration, byte? QuarterNumber, byte? TimeoutDuration, Guid VisitorsId, Guid HostsId, Guid? PrepEncoderId);
public record MatchUpdatePrepDto(byte? QuarterDuration, byte? QuarterNumber, byte? TimeoutDuration, Guid? VisitorsId, Guid? HostsId, Guid? PrepEncoderId, bool? HasStarted);
public record MatchUpdatePlayDto(ulong? VisitorsScore, ulong? HostsScore, bool? IsFinished, Guid? WinnerId);
public record MatchUpdateListDto(IEnumerable<Guid> IncomingEntities, IEnumerable<Guid> LeavingEntities);