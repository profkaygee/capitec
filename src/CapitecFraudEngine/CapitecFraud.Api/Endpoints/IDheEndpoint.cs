namespace CapitecFraud.Api.Endpoints;

public interface ICapitecFraudEndpoint
{
    void RegisterEndpointRoutes(IEndpointRouteBuilder app, string prefix);
}