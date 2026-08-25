public interface IFolderService
{
    public Task<ServiceResult<FolderResponse>> GetById(UserResponse user, Guid id);
    public Task<ServiceResult<FolderResponse>> Create(UserResponse user, CreateFolderRequest? data);
    public Task<ServiceResult<FolderResponse>> Edit(UserResponse user, Guid id, EditFolderRequest data);
    public Task<ServiceResult<bool>> Delete(UserResponse user, Guid id);
}