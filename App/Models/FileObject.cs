public class FileObject
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Name { get; set; } = "";
    public long Size { get; set; } = 0;
    public required Guid ParentId { get; set; }
    public string StoragePath { get; set; } = "";
    public required UserCreatedBy CreatedBy { get; set; }
}
