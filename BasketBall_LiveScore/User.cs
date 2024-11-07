﻿using System.ComponentModel.DataAnnotations;

namespace BasketBall_LiveScore;
public class User
{

    public enum Role
    {
        None,
        Admin
    }


    public int ID { get; set; }
    [Required]
    [MaxLength(250)]
    public string Username { get; set; } = string.Empty;
    [Required]
    [MaxLength(50)]
    public string Password { get; set; } = string.Empty;
    [Required]
    public string Email { get; set; } = string.Empty;
    [Required]
    public Role Permission { get; set; } = User.Role.None;
}
