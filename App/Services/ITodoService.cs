public interface ITodoService
{
    Task<ServiceResult<List<TodoResponse>>> GetAll(UserResponse user);
    Task<ServiceResult<TodoResponse>> Create(UserResponse user, TodoRequest data);
    Task<ServiceResult<TodoResponse>> Edit(UserResponse user, int id, TodoRequest data);
    Task<ServiceResult<bool>> Delete(UserResponse user, int id);
}