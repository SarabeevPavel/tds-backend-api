using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/owner")]
[Authorize(Roles = "Owner")]
public class OwnerController(IOwnerService ownerService) : ControllerBase
{
    // users
    [HttpDelete("users/{id:int}")]
    public async Task<ActionResult<User?>> DeleteUser(int id)
    {
        var result = await ownerService.DeleteUser(id);
        return this.ToActionResult(result);
    }

    // todos
    [HttpGet("todos")]
    public async Task<ActionResult<List<Todo>>> GetAllTodos()
    {
        var entries = await ownerService.GetAllTodos();
         return this.ToActionResult(entries);
    }

    [HttpPost("todos")]
    public async Task<ActionResult<Todo>> CreateTodo([FromBody] OwnerCreateTodoRequest data)
    {
        var todo = await ownerService.CreateTodo(data);
       return this.ToActionResult(todo);
    }

    [HttpPatch("todos/{id:int}")]
    public async Task<ActionResult<Todo>> EditTodo(int id, TodoRequest data)
    {
        var todo = await ownerService.EditTodo(id, data);
       return this.ToActionResult(todo);
    }

    [HttpDelete("todos/{id:int}")]
    public async Task<ActionResult<Todo?>> DeleteTodo(int id)
    {
        var result = await ownerService.DeleteTodo(id);
        return this.ToActionResult(result);
    }

    // folders
    [HttpPost("folders/{id:int}")]
    public async Task<ActionResult<User?>> CreateUserRootFolder(int id)
    {
        var result = await ownerService.CreateUserRootFolder(id);
        return this.ToActionResult(result);
    }

    [HttpDelete("folders/{id:guid}")]
    public async Task<ActionResult<FolderObject?>> Delete(Guid id)
    {
        var result = await ownerService.DeleteFolder(id);
        return this.ToActionResult(result);
    }
}