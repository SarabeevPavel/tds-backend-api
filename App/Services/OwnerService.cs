using Microsoft.EntityFrameworkCore;

public class OwnerService(AppDbContext db, IFolderService folderService) : IOwnerService
{
    // todos
    public async Task<ServiceResult<List<Todo>>> GetAllTodos()
    {
        var entries = await db.Todos.AsNoTracking().ToListAsync();
        return ServiceResult<List<Todo>>.Ok(entries);
    }

    public async Task<ServiceResult<Todo>> CreateTodo(OwnerCreateTodoRequest data)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == data.UserId);
        if (user is null)
        {
            return ServiceResult<Todo>.Fail(ServiceError.NotFound, "User not found");
        }
        ;
        Todo todo = new Todo { Title = data.Title, IsDone = data.IsDone ?? false, CreatedBy = new UserCreatedBy(user.Id, user.Username) };
        await db.Todos.AddAsync(todo);
        await db.SaveChangesAsync();
        return ServiceResult<Todo>.Ok(todo);
    }

    public async Task<ServiceResult<Todo>> EditTodo(int id, TodoRequest data)
    {
        var todo = await db.Todos.FirstOrDefaultAsync(t => t.Id == id);
        if (todo is null)
        {
            return ServiceResult<Todo>.Fail(ServiceError.NotFound, "Todo not found");
        }
        todo.Title = data.Title;
        todo.IsDone = data.IsDone ?? todo.IsDone;
        await db.SaveChangesAsync();
        return ServiceResult<Todo>.Ok(todo);
    }

    public async Task<ServiceResult<Todo>> DeleteTodo(int id)
    {
        var todo = await db.Todos.FirstOrDefaultAsync(t => t.Id == id);
        if (todo is null)
        {
            return ServiceResult<Todo>.Fail(ServiceError.NotFound, "Todo not found");
        }
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return ServiceResult<Todo>.Ok(todo);
    }

    // folders
    public async Task<ServiceResult<User>> CreateUserRootFolder(int userId)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
        {
            return ServiceResult<User>.Fail(ServiceError.NotFound, "User not found");
        }

        var _path = Path.Combine(Directory.GetCurrentDirectory(), "uploads", user.Id.ToString());

        bool isDirectoryExist = Directory.Exists(_path);
        
        // if folder obj is already in DB
        if (user.RootFolderId is not null)
        {
            // if physical users's folder (PUF) also is already exist 
            if (isDirectoryExist)
            {
                return ServiceResult<User>.Fail(ServiceError.Conflict, "This user already has root folder");
            }
            else // if no PUF
            {
                Directory.CreateDirectory(_path);
            }
        }
        else // if no folder object in DB
        {
            var folder = await folderService.Create(new UserResponse(user.Id, user.Username, user.Role, user.RootFolderId), null);
            if (folder.Data is null)
            {
                return ServiceResult<User>.Fail(ServiceError.BadRequest, "Couldn't create folder");
            }

            user.RootFolderId = folder.Data.Id;

            // additionally - if also no PUF
            if (!isDirectoryExist)
            {
                Directory.CreateDirectory(_path);
            }
            await db.SaveChangesAsync();
        }
    
        return ServiceResult<User>.Ok(user);
    }
    
     public async Task<ServiceResult<FolderObject>> DeleteFolder(Guid id)
    {
        var folder = await db.Folders.FindAsync(id);
        if (folder is null)
        {
            return ServiceResult<FolderObject>.Fail(ServiceError.NotFound, "Folder not found");        
        }
        db.Folders.Remove(folder);
        await db.SaveChangesAsync();
        return ServiceResult<FolderObject>.Ok(folder);;
    }

    // users
    public async Task<ServiceResult<User>> DeleteUser(int id)
    {
        var user = await db.Users.FindAsync(id);
        if (user is null)
        {
            return ServiceResult<User>.Fail(ServiceError.NotFound, "User not found");
        }

        var folders = await db.Folders.Where(f => f.CreatedBy.Id == id).ToListAsync();
        var files = await db.Files.Where(f => f.CreatedBy.Id == id).ToListAsync();
        db.Files.RemoveRange(files);
        db.Folders.RemoveRange(folders);
        db.Users.Remove(user);

        await db.SaveChangesAsync();

        var _path = Path.Combine(Directory.GetCurrentDirectory(), "uploads", user.Id.ToString());
        var isDirectoryExist = Directory.Exists(_path);
         
        if (isDirectoryExist)
        {
            Directory.Delete(_path, recursive: true);
        }

        return ServiceResult<User>.Ok(user);
    }
}