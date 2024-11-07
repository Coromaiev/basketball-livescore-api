using System.ComponentModel.DataAnnotations;

namespace BasketBall_LiveScore;
public class Match
{
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
    public int? VisitorsID { get; set; }
    [Required]
    public int? HostsID { get; set; }
    [Required]
    public User? PrepEncoder { get; set; }
    [Required]
    public User? PlayEncoder { get; set; }
    [Required]
    public List<MatchEvent> Events { get; set; } = new();
}
