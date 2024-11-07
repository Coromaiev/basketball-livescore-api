using System.ComponentModel.DataAnnotations;

namespace BasketBall_LiveScore;
public class Player
{
    public int Id { get; set; }
    [MaxLength(70)]
    public string FirstName { get; set; } = string.Empty;
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
    public int TeamId { get; set; }
    public byte Number { get; set; }
    public Team? Team { get; set; }

}
