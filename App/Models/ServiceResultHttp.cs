using Microsoft.AspNetCore.Mvc;

public static class ServiceResultHttp
{
    public static ActionResult ToActionResult<T>(this ControllerBase controller, ServiceResult<T> result)
    {
        return result.Error switch
        {
            ServiceError.None => controller.Ok(result.Data),
            ServiceError.NotFound => controller.NotFound(result.Message),
            ServiceError.Conflict => controller.Conflict(result.Message),
            ServiceError.Forbidden => controller.StatusCode(StatusCodes.Status403Forbidden, result.Message),
            ServiceError.Unauthorized => controller.Unauthorized(result.Message),
            _ => controller.BadRequest(result.Message),
        };
    }
}
