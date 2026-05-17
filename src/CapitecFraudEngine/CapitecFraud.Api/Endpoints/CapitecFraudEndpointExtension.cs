namespace CapitecFraud.Api.Endpoints;

public static class CapitecFraudEndpointExtension
{
    public static void ConfigureEndpoints(this IEndpointRouteBuilder app, string apiVersion = "v1")
    {
        var endpointTypes = typeof(CapitecFraudEndpointExtension).Assembly.GetTypes()
            .Where(t => t.IsClass && typeof(ICapitecFraudEndpoint).IsAssignableFrom(t) && !t.IsAbstract);

        foreach (var type in endpointTypes)
        {
            var endpoint = Activator.CreateInstance(type) as ICapitecFraudEndpoint;
            endpoint?.RegisterEndpointRoutes(app, $"api/v{apiVersion}");
        }
    }
}