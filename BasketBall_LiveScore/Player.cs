using System.ComponentModel.DataAnnotations;

namespace BasketBall_LiveScore;
public class Player
{
    public int Id { get; set; }
    [Required]
    [MaxLength(70)]
    public string FirstName { get; set; } = string.Empty;
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
    [Required]
    public int TeamId { get; set; }
    [Required]
    public byte Number { get; set; }
    [Required]
    public Team? Team { get; set; }

}
