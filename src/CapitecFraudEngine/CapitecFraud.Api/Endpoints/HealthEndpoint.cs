namespace CapitecFraud.Api.Endpoints;

public class HealthEndpoint:ICapitecFraudEndpoint
{
    public void RegisterEndpointRoutes(IEndpointRouteBuilder app, string prefix)
    {
        app.MapGet($"{prefix}/health", () =>
        {
            return Results.Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow
            });
        }).WithName("HealthCheck");
    }
}