namespace CapitecFraud.Application.Common.Responses;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }

    public static ApiResponse<T> SuccessResponse(T data, int statusCode = 200, string message = null) =>
        new()
        {
            Success = true,
            StatusCode = statusCode,
            Data = data,
            Message = message
        };

    public static ApiResponse<T> ErrorResponse(string message, int statusCode) =>
        new()
        {
            Success = false,
            StatusCode = statusCode,
            Message = message,
            Data = default
        };
}