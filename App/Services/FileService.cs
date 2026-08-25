public class FileService(AppDbContext db) : IFileService
{
    public async Task<ServiceResult<FileResponse>> Upload(UserResponse user, UploadFileRequest data)
    {
        if (data.File is null || data.File.Length == 0)
        {
            return ServiceResult<FileResponse>.Fail(ServiceError.BadRequest, "No file uploaded");
        }

        var parentId = data.ParentId ?? user.RootFolderId;
        if (parentId is null)
        {
            return ServiceResult<FileResponse>.Fail(ServiceError.BadRequest, "User has no root folder");
        }

        var folder = await db.Folders.FindAsync(parentId);
        if (folder is null || folder.CreatedBy.Id != user.Id)
        {
            return ServiceResult<FileResponse>.Fail(ServiceError.NotFound, "Folder not found");
        }

        var dir = Path.Combine(Directory.GetCurrentDirectory(), "uploads", user.Id.ToString());
        Directory.CreateDirectory(dir);

        var file = new FileObject
        {
            Name = Path.GetFileName(data.File.FileName),
            Size = data.File.Length,
            ParentId = parentId.Value,
            CreatedBy = new UserCreatedBy(user.Id, user.Username),
        };

        var fullPath = Path.Combine(dir, file.Id.ToString());
        file.StoragePath = Path.Combine("uploads", user.Id.ToString(), file.Id.ToString());

        await using (var stream = File.Create(fullPath))
        {
            await data.File.CopyToAsync(stream);
        }

        await db.Files.AddAsync(file);
        await db.SaveChangesAsync();

        return ServiceResult<FileResponse>.Ok(new FileResponse(file.Id, file.Name, file.Size, file.ParentId, file.CreatedBy));
    }

    public async Task<ServiceResult<FileResponse>> GetById(UserResponse user, Guid id)
    {
        var file = await db.Files.FindAsync(id);
        if (file is null || file.CreatedBy.Id != user.Id)
        {
            return ServiceResult<FileResponse>.Fail(ServiceError.NotFound, "File not found");
        }

        return ServiceResult<FileResponse>.Ok(new FileResponse(file.Id, file.Name, file.Size, file.ParentId, file.CreatedBy));
    }

    public async Task<ServiceResult<bool>> Delete(UserResponse user, Guid id)
    {
        var file = await db.Files.FindAsync(id);
        if (file is null || file.CreatedBy.Id != user.Id)
        {
            return ServiceResult<bool>.Fail(ServiceError.NotFound, "File not found");
        }

        db.Files.Remove(file);
        await db.SaveChangesAsync();

        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), file.StoragePath);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return ServiceResult<bool>.Ok(true);
    }
}

