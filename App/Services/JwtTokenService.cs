using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

public class JwtTokenService(IConfiguration config, AppDbContext db, IMemoryCache cache) : IJwtTokenService
{
    public async Task<ServiceResult<JwtTokenResponse>> CreateToken(LoginRequest credentials)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == credentials.Username);
        if (user is null)
        {
            return ServiceResult<JwtTokenResponse>.Fail(ServiceError.Unauthorized, "Unauthorized");
        }

        var hasher = new PasswordHasher<User>();
        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, credentials.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            return ServiceResult<JwtTokenResponse>.Fail(ServiceError.Unauthorized, "Incorrect credentials");
        }
            

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, credentials.Username),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("root_folder_id", user.RootFolderId!.ToString()!),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

        return ServiceResult<JwtTokenResponse>.Ok(new JwtTokenResponse (accessToken)); 
    }

    public ServiceResult<bool> ExpireToken(ClaimsPrincipal user)
    {
        var jti = user.FindFirstValue(JwtRegisteredClaimNames.Jti);
        if (jti is null)
        {
            return ServiceResult<bool>.Fail(ServiceError.BadRequest, "Session has already expired");
        }

        var expRaw = user.FindFirstValue(JwtRegisteredClaimNames.Exp);
        if (!long.TryParse(expRaw, out var expUnix))
        {
            return ServiceResult<bool>.Fail(ServiceError.BadRequest, "Session has already expired");
        }

        var exp = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;


        if (exp > DateTime.UtcNow)
        {
            cache.Set($"jwt:revoked:{jti}", true, exp);
        }
        
        return ServiceResult<bool>.Ok(true);
        
    }

    public ServiceResult<int> GetUserId(ClaimsPrincipal user)
    {
        var raw = user.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = int.TryParse(raw, out var id);

        if (!result)
        {
            return ServiceResult<int>.Fail(ServiceError.Unauthorized, "Unauthorized");
        }

        return ServiceResult<int>.Ok(id);
    }

    public ServiceResult<UserResponse> GetUser(ClaimsPrincipal user)
    {
        var id = GetUserId(user);
        if (id.Error != ServiceError.None)
        {
            return ServiceResult<UserResponse>.Fail(ServiceError.Unauthorized, "Unauthorized");
        }
        var rootRaw = user.FindFirstValue("root_folder_id");
        var username = user.FindFirstValue(ClaimTypes.Name) ?? "";
        var role = user.FindFirstValue(ClaimTypes.Role) ?? "";
        Guid? rootFolderId = Guid.TryParse(rootRaw, out var g) ? g : null;

        return ServiceResult<UserResponse>.Ok(new UserResponse(id.Data, username, role, rootFolderId ));
    }
}