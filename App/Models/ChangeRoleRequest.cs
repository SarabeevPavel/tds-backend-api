using System.ComponentModel.DataAnnotations;

public record ChangeRoleRequest(
    [Required]
    [MinLength(1)]
    string Role
);
