using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/folders")]
[Authorize]
public class FolderController(IJwtTokenService jwtTokenService, IFolderService folderService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetById(Guid id)
    {
        var user = jwtTokenService.GetUser(User);
        if (user.Data is null) return this.ToActionResult(user);
        var result = await folderService.GetById(user.Data, id);
        return this.ToActionResult(result);
    }
    
    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateFolderRequest data)
    {
        var user = jwtTokenService.GetUser(User);
       if (user.Data is null) return this.ToActionResult(user);
        var result = await folderService.Create(user.Data, data);
        return this.ToActionResult(result);
    }

    [HttpPatch("{id:guid}")]
    public async Task<ActionResult> Edit(Guid id, [FromBody] EditFolderRequest body)
    {
        var user = jwtTokenService.GetUser(User);
        if (user.Data is null) return this.ToActionResult(user);
        var result = await folderService.Edit(user.Data, id, body);
        return this.ToActionResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var user = jwtTokenService.GetUser(User);
        if (user.Data is null) return this.ToActionResult(user);
        var result = await folderService.Delete(user.Data, id);
        return this.ToActionResult(result);
    }
}