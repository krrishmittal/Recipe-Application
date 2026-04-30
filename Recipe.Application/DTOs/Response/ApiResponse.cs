namespace Recipe.Application.DTOs.Response;

/// <summary>
/// Represents an API error returned to clients.
/// </summary>
public class ApiError
{
    /// <summary>
    /// Gets or sets the response or error code.
    /// </summary>
    public int Code { get; set; }
    /// <summary>
    /// Gets or sets the response message.
    /// </summary>
    public string Message { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the error location.
    /// </summary>
    public string Location { get; set; } = string.Empty;
}

/// <summary>
/// Represents the standard API response envelope.
/// </summary>
public class ApiResponse<T>
{
    /// <summary>
    /// Gets or sets whether the operation succeeded.
    /// </summary>
    public bool Success { get; set; }
    /// <summary>
    /// Gets or sets the response message.
    /// </summary>
    public string Message { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the response payload.
    /// </summary>
    public T? Data { get; set; }
    /// <summary>
    /// Gets or sets the response errors.
    /// </summary>
    public IEnumerable<ApiError>? Errors { get; set; }

    /// <summary>
    /// Creates a successful API response.
    /// </summary>
    public static ApiResponse<T> Ok(T data, string message = "Operation completed successfully.") =>
        new() { Success = true, Message = message, Data = data };

    /// <summary>
    /// Creates a failed API response.
    /// </summary>
    public static ApiResponse<T> Fail(string message, int code, string location) =>
        new()
        {
            Success = false,
            Message = message,
            Errors = new[]
            {
                new ApiError { Code = code, Message = message, Location = location }
            }
        };

    /// <summary>
    /// Creates a failed API response with multiple errors.
    /// </summary>
    public static ApiResponse<T> Fail(string message, IEnumerable<ApiError> errors) =>
        new()
        {
            Success = false,
            Message = message,
            Errors = errors
        };
}
