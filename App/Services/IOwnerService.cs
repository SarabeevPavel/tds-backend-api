public interface IOwnerService
{
    // Todos
    Task<ServiceResult<List<Todo>>> GetAllTodos();
    Task<ServiceResult<Todo>> CreateTodo(OwnerCreateTodoRequest data);
    Task<ServiceResult<Todo>> EditTodo(int id, TodoRequest data);
    Task<ServiceResult<Todo>> DeleteTodo(int id);

    // folders
    Task<ServiceResult<User>> CreateUserRootFolder(int userId);
    Task<ServiceResult<FolderObject>> DeleteFolder(Guid id);

    // users
    Task<ServiceResult<User>> DeleteUser(int id);
}