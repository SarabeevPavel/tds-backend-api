using System.ComponentModel.DataAnnotations;

public record OwnerCreateTodoRequest(
    [Required]
    [Range(1, int.MaxValue)]
    int UserId,
    [Required(ErrorMessage = "Title is required")]
    [MinLength(3)]
    string Title,
    bool? IsDone

);
