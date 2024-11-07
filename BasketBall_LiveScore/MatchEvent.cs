using System.ComponentModel.DataAnnotations;

namespace BasketBall_LiveScore;
public abstract class MatchEvent
{
    [Required]
    public decimal Time { get; set; }
    [Required]
    public byte QuarterNumber { get; set; }
    [Required]
    public int MatchID { get; set; }
    [Required]
    public Match? Match { get; set; }
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
    public required FaultType Type { get; set; }
    [Required]
    public required Player FaultyPlayer { get; set; }

    public string FaultTypeToString(FaultType type)
    {
        switch(type) {
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
    public int InvokerID { get; set; }
    [Required]
    public required Team Invoker { get; set; }
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
    public required Points Score { get; set; }
    [Required]
    public required Player Scorer { get; set; }
}

public class PlayerChange : MatchEvent
{
    [Required]
    public required Player LeavingPlayer { get; set; }
    [Required]
    public required Player ReplacingPlayer { get; set; }
}
