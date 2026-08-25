public interface IUserService
{
   Task<ServiceResult<List<UserResponse>>> GetAll();
   Task<ServiceResult<UserResponse>> Get(int id);
   Task<ServiceResult<UserResponse>> Create(RegisterRequest credentials);
   Task<ServiceResult<UserResponse>> Edit(int id, EditUserRequest data);
   Task<ServiceResult<UserResponse>> ChangeRole(int id, string role);
}
