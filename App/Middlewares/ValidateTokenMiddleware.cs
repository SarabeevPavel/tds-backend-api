using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Caching.Memory;

public class ValidateTokenMiddleware(RequestDelegate next, IMemoryCache cache)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var jti = context.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
        if (jti != null && cache.TryGetValue($"jwt:revoked:{jti}", out _))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }
        await next(context);
    }
}