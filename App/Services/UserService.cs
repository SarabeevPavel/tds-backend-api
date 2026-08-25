using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class UserService(AppDbContext db, IFolderService folderService) : IUserService
{
    public async Task<ServiceResult<List<UserResponse>>> GetAll()
    {
        var entries = await db.Users.AsNoTracking().Select(u => new UserResponse(u.Id, u.Username, u.Role, u.RootFolderId)).ToListAsync();
        return ServiceResult<List<UserResponse>>.Ok(entries);
    }

    public async Task<ServiceResult<UserResponse>> Get(int id)
    {
        var user = await db.Users.FindAsync(id);
        if (user is null)
        {
            return ServiceResult<UserResponse>.Fail(ServiceError.NotFound, "User not found");

        }
        return ServiceResult<UserResponse>.Ok(new UserResponse(user.Id, user.Username, user.Role, user.RootFolderId));

    }
    
    public async Task<ServiceResult<UserResponse>> Create(RegisterRequest data)
    {
        var exist = await db.Users.AnyAsync(u => u.Username == data.Username);
        if (exist)
        {
            return ServiceResult<UserResponse>.Fail(ServiceError.Conflict, "User is already exist");
        }

        var hasher = new PasswordHasher<User>();
        var user = new User { Username = data.Username };
        user.PasswordHash = hasher.HashPassword(user, data.Password);

        db.Users.Add(user);
        await db.SaveChangesAsync();

        var userResponse = new UserResponse(user.Id, user.Username, user.Role, user.RootFolderId);
        var rootFolder = await folderService.Create(userResponse, null);

        if (rootFolder.Data is null)
        {
            return ServiceResult<UserResponse>.Ok(userResponse, "User created successfully, but root folder has not created! Report to admin please");
        }

        user.RootFolderId = rootFolder.Data.Id;

        await db.SaveChangesAsync();

        return ServiceResult<UserResponse>.Ok(userResponse);
    }

    public async Task<ServiceResult<UserResponse>> Edit(int id, EditUserRequest data)
    {
        var user = await db.Users.FindAsync(id);
        if (user is null)
        {
            return ServiceResult<UserResponse>.Fail(ServiceError.NotFound, "User not found");
        }

        if (data.Username is not null) user.Username = data.Username;
        if (data.Password is not null)
            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, data.Password);
        if (data.Role is not null) user.Role = data.Role;

        await db.SaveChangesAsync();
        return ServiceResult<UserResponse>.Ok(new UserResponse(user.Id, user.Username, user.Role, user.RootFolderId));
    }

    public async Task<ServiceResult<UserResponse>> ChangeRole(int id, string role)
    {
        var user = await db.Users.FindAsync(id);
        if (user is null) {
            return ServiceResult<UserResponse>.Fail(ServiceError.NotFound, "User not found");
        }

        user.Role = role;
        await db.SaveChangesAsync();
        return ServiceResult<UserResponse>.Ok(new UserResponse(user.Id, user.Username, user.Role, user.RootFolderId));
    }  
}
