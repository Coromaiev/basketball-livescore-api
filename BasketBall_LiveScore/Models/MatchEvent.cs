using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BasketBall_LiveScore.Models;
public abstract class MatchEvent
{
    [Required]
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    public TimeSpan Time { get; set; }
    [Required]
    public byte QuarterNumber { get; set; }
    [Required]
    public Guid MatchId { get; set; }
    [Required]
    public Match Match { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FaultType
{
    P0,
    P1,
    P2,
    P3
}

public class Fault : MatchEvent
{
    [Required]
    public FaultType Type { get; set; }
    [Required]
    public Guid FaultyPlayerId { get; set; }
    [Required]
    public Player FaultyPlayer { get; set; }

    //public string FaultTypeToString(FaultType type)
    //{
    //    switch (type)
    //    {
    //        case FaultType.P0:
    //            return "P0";
    //        case FaultType.P1:
    //            return "P1";
    //        case FaultType.P2:
    //            return "P2";
    //        case FaultType.P3:
    //            return "P3";
    //        default:
    //            return "";
    //    }
    //}
}

public class TimeOut : MatchEvent
{
    [Required]
    public Guid InvokerId { get; set; } 
    [Required]
    public Team Invoker { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Points
{
    One = 1,
    Two = 2,
    Three = 3,
}

public class ScoreChange : MatchEvent
{
    [Required]
    public Points Score { get; set; }
    [Required]
    public Guid ScorerId { get; set; }
    [Required]
    public Player Scorer { get; set; }
}

public class PlayerChange : MatchEvent
{
    [Required]
    public Guid LeavingPlayerId { get; set; }
    [Required]
    public Player LeavingPlayer { get; set; }
    [Required]
    public Guid ReplacingPlayerId { get; set; }
    [Required]
    public Player ReplacingPlayer { get; set; }
}
public record MatchEventUpdateDto(TimeSpan? Time, byte? QuarterNumber);
public abstract record MatchEventCreateDto(TimeSpan Time, byte QuarterNumber, Guid MatchId);
public abstract record MatchEventDto(Guid Id, TimeSpan Time, byte QuarterNumber, Guid MatchId);
public record FaultDto(Guid Id, TimeSpan Time, byte QuarterNumber, Guid MatchId, Guid FaultyPlayerId, FaultType FaultType)
    : MatchEventDto(Id, Time, QuarterNumber, MatchId);
public record FaultCreateDto(TimeSpan Time, byte QuarterNumber, Guid MatchId, Guid FaultyPlayerId, FaultType FaultType)
    : MatchEventCreateDto(Time, QuarterNumber, MatchId);
public record ScoreChangeDto(Guid Id, TimeSpan Time, byte QuarterNumber, Guid MatchId, Guid ScorerId, Points Points)
    : MatchEventDto(Id, Time, QuarterNumber, MatchId);
public record ScoreChangeCreateDto(TimeSpan Time, byte QuarterNumber, Guid MatchId, Guid ScorerId, Points Points)
    : MatchEventCreateDto(Time, QuarterNumber, MatchId);
public record TimeOutDto(Guid Id, TimeSpan Time, byte QuarterNumber, Guid MatchId, Guid InvokerId)
    : MatchEventDto(Id, Time, QuarterNumber, MatchId);
public record TimeOutCreateDto(TimeSpan Time, byte QuarterNumber, Guid MatchId, Guid InvokerId)
    : MatchEventCreateDto(Time, QuarterNumber, MatchId);
public record PlayerChangeDto(Guid Id, TimeSpan Time, byte QuarterNumber, Guid MatchId, Guid LeavingPlayerId, Guid ReplacingPlayerId)
    : MatchEventDto(Id, Time, QuarterNumber, MatchId);
public record PlayerChangeCreateDto(TimeSpan Time, byte QuarterNumber, Guid MatchId, Guid LeavingPlayerId, Guid ReplacingPlayerId)
    : MatchEventCreateDto(Time, QuarterNumber, MatchId);