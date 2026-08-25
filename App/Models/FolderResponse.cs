public enum FolderEntryType
{
    Folder,
    File,

}
public record FolderEntry(FolderEntryType Type, Guid Id, string? Name, Guid? ParentId, long? Size,  UserCreatedBy CreatedBy);
public record FolderResponse(Guid Id, string? Name, Guid? ParentId, UserCreatedBy CreatedBy, List<FolderEntry>? Entries);
