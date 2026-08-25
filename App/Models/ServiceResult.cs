public enum ServiceError
{
    None,
    NotFound,
    BadRequest,
    Conflict,
    Forbidden,
    Unauthorized,
}

public record ServiceResult<T>(T? Data, ServiceError Error, string? Message)
{
    public static ServiceResult<T> Ok(T data, string? message = null) => new(data, ServiceError.None, message);

    public static ServiceResult<T> Fail(ServiceError error, string? message = null) =>
        new(default, error, message);
}
