using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/users")]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<List<UserResponse>>> GetAll()
    {
        var data = await userService.GetAll();
        return this.ToActionResult(data);
    }

    [HttpPatch("{id:int}")]
    [Authorize(Policy = "OwnerOnly")]
    public async Task<ActionResult<UserResponse>> Edit(int id, [FromBody] EditUserRequest body)
    {
       var user = await userService.Edit(id, body);
       return this.ToActionResult(user);
    }

    [HttpPatch("role/{id:int}")]
    [Authorize(Policy = "OwnerOnly")]
    public async Task<ActionResult> ChangeRole(int id, [FromBody] ChangeRoleRequest body)
    {
        var result = await userService.ChangeRole(id, body.Role);
        return this.ToActionResult(result);
    }  
}
