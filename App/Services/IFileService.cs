public interface IFileService
{
    Task<ServiceResult<FileResponse>> Upload(UserResponse user, UploadFileRequest fileData);
    Task<ServiceResult<FileResponse>> GetById(UserResponse user, Guid id);
    Task<ServiceResult<bool>> Delete(UserResponse user, Guid id);
}
