using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

public record EditUserRequest(
    [Optional]
    [MinLength(2)]
    [MaxLength(20)]
    string? Username,
    [Optional]
    [MinLength(3)]
    [MaxLength(20)]
    string? Password,
    [Optional]
    [MinLength(1)]
    string? Role
);
