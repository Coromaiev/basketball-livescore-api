namespace BasketBall_LiveScore;
public class Match
{
    public byte QuarterDuration { get; set; }
    public byte NumberOfQuarters { get; set; }
    public byte TimeOutDuration { get; set; }
    public Team? Visitors { get; set; }
    public Team? Hosts { get; set; }
    public int? VisitorsID { get; set; }
    public int? HostsID { get; set; }
    public User? PrepEncoder { get; set; }
    public User? PlayEncoder { get; set; }
    public List<MatchEvent> Events { get; set; } = new();
}
