using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

public class TodoService(AppDbContext db) : ITodoService
{
    public async Task<ServiceResult<List<TodoResponse>>> GetAll(UserResponse user)
    {
        var entries = await db.Todos.AsNoTracking().Where(t => t.CreatedBy.Id == user.Id).Select(t => new TodoResponse(t.Id, t.Title, t.IsDone)).ToListAsync();
        return ServiceResult<List<TodoResponse>>.Ok(entries);
    }

    public async Task<ServiceResult<TodoResponse>> Create(UserResponse user, TodoRequest data)
    {
        var newEntry = new Todo { Title = data.Title, IsDone = data.IsDone ?? false, CreatedBy = new UserCreatedBy(user.Id, user.Username) };
        db.Todos.Add(newEntry);
        await db.SaveChangesAsync();
        return ServiceResult<TodoResponse>.Ok(new TodoResponse(newEntry.Id, newEntry.Title, newEntry.IsDone));
         
    }

    public async Task<ServiceResult<TodoResponse>> Edit(UserResponse user, int id, TodoRequest data)
    {
        var todo = await db.Todos.FirstOrDefaultAsync(t => t.CreatedBy.Id == user.Id && t.Id == id);
        if (todo is null)
        {
            return ServiceResult<TodoResponse>.Fail(ServiceError.NotFound, "Todo not found");
        }
        todo.Title = data.Title;
        todo.IsDone = data.IsDone ?? todo.IsDone;
        await db.SaveChangesAsync();
        return ServiceResult<TodoResponse>.Ok(new TodoResponse(todo.Id, todo.Title, todo.IsDone));
    }

    public async Task<ServiceResult<bool>> Delete(UserResponse user, int id)
    {
        var todo = await db.Todos.FirstOrDefaultAsync(t => t.CreatedBy.Id == user.Id && t.Id == id);
        if (todo is null)
        {
            return ServiceResult<bool>.Fail(ServiceError.NotFound, "Todo not found");
        }
        db.Todos.Remove(todo);
        var result = await db.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }
}