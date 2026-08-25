public class FolderObject
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Name { get; set; } = null;
    public Guid? ParentId{ get; set; } = null;
    public required UserCreatedBy CreatedBy { get; set; }
    
}