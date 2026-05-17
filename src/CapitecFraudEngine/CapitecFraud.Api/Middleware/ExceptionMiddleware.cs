using System.Net;
using System.Text.Json;

namespace CapitecFraud.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var correlationId = context.Items["X-Correlation-ID"]?.ToString();

            _logger.LogError(ex,
                "Unhandled exception occurred. CorrelationId: {CorrelationId}",
                correlationId);

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new
            {
                error = "An unexpected error occurred",
                correlationId
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}