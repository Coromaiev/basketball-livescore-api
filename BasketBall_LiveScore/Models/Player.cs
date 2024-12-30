using System.ComponentModel.DataAnnotations;

namespace BasketBall_LiveScore.Models;
public class Player
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [MaxLength(70)]
    [Required]
    public string FirstName { get; set; } = string.Empty;
    [MaxLength(100)]
    [Required]
    public string LastName { get; set; } = string.Empty;
    public Guid? TeamId { get; set; }
    public byte? Number { get; set; }
    public Team? Team { get; set; } = null;

}

public record PlayerDto(Guid Id, string FirstName, string LastName, Guid? TeamId, byte? Number);
public record PlayerCreateDto(string FirstName, string LastName, Guid? TeamId, byte? Number);
public record PlayerUpdateDto(string? FirstName, string? LastName, Guid? TeamId, byte? Number);
