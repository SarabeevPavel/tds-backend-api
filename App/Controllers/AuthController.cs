using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/auth")]
public class AuthController(IJwtTokenService jwtTokenService, IUserService userService) : ControllerBase
{
    [HttpGet("public")]
    [AllowAnonymous]
    public ActionResult Ping() => Ok(new { ok = true });

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserResponse>> Get()
    {
        var id = jwtTokenService.GetUserId(User);
        if (id.Error != ServiceError.None)
        {
            return this.ToActionResult(id);
        }
        var user = await userService.Get(id.Data);
        return this.ToActionResult(user);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult> Register([FromBody] RegisterRequest credentials)
    {
       var result = await userService.Create(credentials);
       return this.ToActionResult(result);

    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult> Login([FromBody] LoginRequest credentials)
    {
        var result = await jwtTokenService.CreateToken(credentials);
        return this.ToActionResult(result);
    }

    [HttpPost("logout")]
    [Authorize]
    public ActionResult Logout()
    {
        var result = jwtTokenService.ExpireToken(User);
        return this.ToActionResult(result);
    }
}