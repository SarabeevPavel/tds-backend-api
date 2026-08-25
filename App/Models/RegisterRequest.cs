
using System.ComponentModel.DataAnnotations;

public record RegisterRequest(
    [Required(ErrorMessage = "username is required")]
    [MinLength(2)]
    [MaxLength(20)]
    string Username,
     [Required(ErrorMessage = "password is required")]
    [MinLength(3)]
    [MaxLength(20)]
    string Password
);
