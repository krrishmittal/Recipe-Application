namespace Server.DTOs.Response
{
    public class ApiError
    {
        public int Code { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public IEnumerable<ApiError>? Errors { get; set; }

        public static ApiResponse<T> Ok(T data, string message = "Operation completed successfully.") =>
            new() { Success = true, Message = message, Data = data };

        public static ApiResponse<object> Fail(
            string message = "Something went wrong.",
            IEnumerable<ApiError>? errors = null) =>
            new() { Success = false, Message = message, Errors = errors };

    }
}
