using System.Security.Claims;

public interface IJwtTokenService
{
    public Task<ServiceResult<JwtTokenResponse>> CreateToken(LoginRequest credentials);
    public ServiceResult<bool> ExpireToken(ClaimsPrincipal user);
    public ServiceResult<int> GetUserId(ClaimsPrincipal user);   
    public ServiceResult<UserResponse> GetUser(ClaimsPrincipal user);   
}