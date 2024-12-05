using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BasketBall_LiveScore.Models;
public abstract class MatchEvent
{
    public ulong Id { get; set; }    
    [Column(TypeName = "decimal(8,2)")]
    [Required]
    public decimal Time { get; set; }
    [Required]
    public byte QuarterNumber { get; set; }
    [Required]
    public ulong MatchID { get; set; }
    [Required]
    public Match? Match { get; set; } = null;
}

public class Fault : MatchEvent
{
    public enum FaultType
    {
        P0,
        P1,
        P2,
        P3
    }

    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public FaultType Type { get; set; }
    [Required]
    public ulong FaultyPlayerId { get; set; }
    [Required]
    public Player FaultyPlayer { get; set; }

    public string FaultTypeToString(FaultType type)
    {
        switch (type)
        {
            case FaultType.P0:
                return "P0";
            case FaultType.P1:
                return "P1";
            case FaultType.P2:
                return "P2";
            case FaultType.P3:
                return "P3";
            default:
                return "";
        }
    }
}

public class TimeOut : MatchEvent
{
    [Required]
    public ulong InvokerID { get; set; } 
    [Required]
    public Team Invoker { get; set; }
}

public class ScoreChange : MatchEvent
{
    public enum Points
    {
        One = 1,
        Two = 2,
        Three = 3,
    }

    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Points Score { get; set; }
    [Required]
    public ulong ScorerId { get; set; }
    [Required]
    public Player Scorer { get; set; }
}

public class PlayerChange : MatchEvent
{
    [Required]
    public ulong LeavingPlayerId { get; set; }
    [Required]
    public Player LeavingPlayer { get; set; }
    [Required]
    public ulong ReplacingPlayerId { get; set; }
    [Required]
    public Player ReplacingPlayer { get; set; }
}
