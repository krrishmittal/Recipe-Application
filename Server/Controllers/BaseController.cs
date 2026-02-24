using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs.Response;

namespace Server.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected IActionResult Success<T>(T data, string message = "Operation completed successfully.") =>
            Ok(ApiResponse<T>.  Ok(data, message));

        protected IActionResult Created<T>(T data, string actionName, object routeValues, string message = "Resource created successfully.") =>
            CreatedAtAction(actionName, routeValues, ApiResponse<T>.Ok(data, message));

        protected IActionResult Fail(string message = "Something went wrong.",int statusCode = 400, string? errorMessage = null, string? location = null)      
        {
            var resolvedLocation = string.IsNullOrEmpty(location) ? HttpContext.Request.Path.ToString() : location;

            var error = new ApiError
            {
                Code = statusCode,
                Message = string.IsNullOrEmpty(errorMessage) ? message : errorMessage,
                Location = resolvedLocation
            };

            var response = ApiResponse<object>.Fail(message, new[] { error });
            return StatusCode(statusCode, response);
        }
    }
}
