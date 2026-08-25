using System.ComponentModel.DataAnnotations;

public record UploadFileRequest(
    [Required]
    IFormFile File,
    [Required(ErrorMessage = "Title is required")]
    Guid? ParentId
);
