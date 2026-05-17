using CapitecFraud.Application.Abstractions;

namespace CapitecFraud.Api.Endpoints;

public class FraudEndpoint:ICapitecFraudEndpoint
{
    public void RegisterEndpointRoutes(IEndpointRouteBuilder app, string prefix)
    {
        // Get fraud transactions
        app.MapPost($"{prefix}/evaluate-transactions", async (ITransactionRepository transactionRepository) =>
        {

        });
        
        // Get fraud transaction by identity
        app.MapGet($"{prefix}/transactions/{{id}}", async (ITransactionRepository transactionRepository) =>
        {

        });
        
        // Get transaction audit trail
        app.MapGet($"{prefix}/audit/{{id}}", async (IAuditRepository auditRepository) =>
        {

        });
    }
}