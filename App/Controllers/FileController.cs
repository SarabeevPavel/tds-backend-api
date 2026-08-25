using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/files")]
[Authorize]
public class FileController(IJwtTokenService jwtTokenService, IFileService fileService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> Upload(IFormFile file, [FromForm] Guid? parentId)
    {
        var user = jwtTokenService.GetUser(User);
        if (user.Data is null) return Unauthorized();
        var result = await fileService.Upload(user.Data, new UploadFileRequest(file, parentId));
        return this.ToActionResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetById(Guid id)
    {
        var user = jwtTokenService.GetUser(User);
        if (user.Data is null) return Unauthorized();
        var file = await fileService.GetById(user.Data, id);
        return this.ToActionResult(file);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var user = jwtTokenService.GetUser(User);
        if (user.Data is null) return Unauthorized();
        var result = await fileService.Delete(user.Data, id);
        return this.ToActionResult(result);
    }
}
