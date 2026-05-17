using CapitecFraud.Application.Common.Responses;

namespace CapitecFraud.Api.Common;

public static class ApiResults
{
    public static IResult Ok<T>(T data, string message = null) =>
        Results.Json(
            ApiResponse<T>.SuccessResponse(data, 200, message),
            statusCode: StatusCodes.Status200OK
        );

    public static IResult Created<T>(string uri, T data, string message = null) =>
        Results.Json(
            ApiResponse<T>.SuccessResponse(data, 201, message),
            statusCode: StatusCodes.Status201Created
        );

    public static IResult BadRequest(string message) =>
        Results.Json(
            ApiResponse<string>.ErrorResponse(message, 400),
            statusCode: StatusCodes.Status400BadRequest
        );

    public static IResult Unauthorized(string message) =>
        Results.Json(
            ApiResponse<string>.ErrorResponse(message, 401),
            statusCode: StatusCodes.Status401Unauthorized
        );

    public static IResult Forbidden(string message) =>
        Results.Json(
            ApiResponse<string>.ErrorResponse(message, 403),
            statusCode: StatusCodes.Status403Forbidden
        );

    public static IResult Conflict(string message) =>
        Results.Json(
            ApiResponse<string>.ErrorResponse(message, 409),
            statusCode: StatusCodes.Status409Conflict
        );

    public static IResult NotFound(string message) =>
        Results.Json(
            ApiResponse<string>.ErrorResponse(message, 404),
            statusCode: StatusCodes.Status404NotFound
        );

    public static IResult InternalServerError(string message = "An unexpected error occurred") =>
        Results.Json(
            ApiResponse<string>.ErrorResponse(message, 500),
            statusCode: StatusCodes.Status500InternalServerError
        );
}