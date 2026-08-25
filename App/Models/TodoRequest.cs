using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

public record TodoRequest(
    [Required]
    [MinLength(1)]
    [MaxLength(200)]
    string Title,
    [Optional]
    bool? IsDone
);
