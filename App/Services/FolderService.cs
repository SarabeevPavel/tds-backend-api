using Microsoft.EntityFrameworkCore;

public class FolderService(AppDbContext db) : IFolderService
{
    public async Task<ServiceResult<FolderResponse>> GetById(UserResponse user, Guid id)
    {
        var folder = await db.Folders.FindAsync(id);
        
        if (folder is null || folder.CreatedBy.Id != user.Id)
        {
            return ServiceResult<FolderResponse>.Fail(ServiceError.NotFound, "Folder not found");
        }
        var folderEntries = await db.Folders.AsNoTracking().Where(f => f.ParentId == folder.Id).Select(f => new FolderEntry(FolderEntryType.Folder, f.Id, f.Name, f.ParentId, null, f.CreatedBy)).ToListAsync();
        var fileEntries = await db.Files.AsNoTracking().Where(f => f.ParentId == folder.Id).Select(f => new FolderEntry(FolderEntryType.File, f.Id, f.Name, f.ParentId, f.Size, f.CreatedBy)).ToListAsync();
        var entries = folderEntries.Concat(fileEntries).ToList();

        return ServiceResult<FolderResponse>.Ok(new FolderResponse(folder.Id, folder.Name, folder.ParentId, folder.CreatedBy, Entries:entries));
    }

    public async Task<ServiceResult<FolderResponse>> Create(UserResponse user, CreateFolderRequest? data)
    {
        
        if (user.RootFolderId is not null && data is not null && data.ParentFolderId is null)
        {
            return ServiceResult<FolderResponse>.Fail(ServiceError.Conflict, "Root folder for this user is already exist");
        }
        
        var folder = new FolderObject { CreatedBy = new UserCreatedBy(user.Id, user.Username) };

        if (data is not null)
        {
            Guid? parentFolderId = null;
            if (data.ParentFolderId is not null)
            {
                var parentFolder = await db.Folders.FindAsync(data.ParentFolderId);
                if (parentFolder is null || parentFolder.CreatedBy.Id != user.Id)
                {
                    return ServiceResult<FolderResponse>.Fail(ServiceError.NotFound, "Parent folder not found");
                }
            
                parentFolderId = parentFolder.Id;
            }


            if (data.Name is not null) folder.Name = data.Name;
            folder.ParentId = parentFolderId;
          
        }

        await db.Folders.AddAsync(folder);

        await db.SaveChangesAsync();

        return ServiceResult<FolderResponse>.Ok(new FolderResponse(folder.Id, folder.Name, folder.ParentId, folder.CreatedBy, []));
    }

    public async Task<ServiceResult<FolderResponse>> Edit(UserResponse user, Guid id, EditFolderRequest data)
    {
        var folder = await db.Folders.FindAsync(id);        
        if (folder is null || folder.CreatedBy.Id != user.Id)
        {
            return ServiceResult<FolderResponse>.Fail(ServiceError.NotFound, "Folder not found");
        }

        if (data.Name is not null) folder.Name = data.Name;
        
        await db.SaveChangesAsync();
        return ServiceResult<FolderResponse>.Ok(new FolderResponse(folder.Id, folder.Name, folder.ParentId, folder.CreatedBy, null));
    }

    public async Task<ServiceResult<bool>> Delete(UserResponse user, Guid id)
    {
        var folder = await db.Folders.FindAsync(id);
        if (folder is null || folder.CreatedBy.Id != user.Id)
        {
            return ServiceResult<bool>.Fail(ServiceError.NotFound, "Folder not found");
        }

        if (folder.ParentId == null || id == user.RootFolderId)
        {
            return ServiceResult<bool>.Fail(ServiceError.Forbidden, "Root folder couldn't be deleted");
        }

        var folderIds = new List<Guid> { id };    
        var fileIds = new List<Guid> {};    
        int i = 0;
        while (i < folderIds.Count)
        {
            var current = folderIds[i++];
            var dFolders = await db.Folders.Where(f => f.ParentId == current && f.CreatedBy.Id == user.Id).Select(f => f.Id).ToListAsync();
            var dFiles = await db.Files.Where(f => f.ParentId == current && f.CreatedBy.Id == user.Id).Select(f => f.Id).ToListAsync();
            folderIds.AddRange(dFolders);
            fileIds.AddRange(dFiles);
        }

    
        var folders = await db.Folders.Where(f => folderIds.Contains(f.Id)).ToListAsync();
        var files = await db.Files.Where(f => fileIds.Contains(f.Id)).ToListAsync();

        db.Files.RemoveRange(files);
        db.Folders.RemoveRange(folders);

        await db.SaveChangesAsync();

        foreach (var file in files)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), file.StoragePath);
            if (File.Exists(path)) File.Delete(path);
        }

        return ServiceResult<bool>.Ok(true);
    }
}