using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/todos")]
public class TodoController(IJwtTokenService jwtTokenService, ITodoService todoService) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<List<TodoResponse>>> GetAll()
    {
        var user = jwtTokenService.GetUser(User);
        if (user.Data is null) return this.ToActionResult(user);
        var entries = await todoService.GetAll(user.Data);
        return this.ToActionResult(entries);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<TodoResponse>> Create([FromBody] TodoRequest data)
    {
        var user = jwtTokenService.GetUser(User);
        if (user.Data is null) return this.ToActionResult(user);
        var entry = await todoService.Create(user.Data, data);
        return this.ToActionResult(entry);
    }

    [HttpPatch("{id:int}")]
    [Authorize]
    public async Task<ActionResult<TodoResponse>> Edit(int id, [FromBody] TodoRequest data)
    {
        var user = jwtTokenService.GetUser(User);
        if (user.Data is null) return this.ToActionResult(user);
        var entry = await todoService.Edit(user.Data, id, data);
        return this.ToActionResult(entry);
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<ActionResult<TodoResponse>> Delete(int id)
    {
        var user = jwtTokenService.GetUser(User);
        if (user.Data is null) return this.ToActionResult(user);
        var result = await todoService.Delete(user.Data, id);
        return this.ToActionResult(result);
    }
        
}