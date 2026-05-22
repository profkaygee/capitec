using CapitecFraud.Api.Common;
using CapitecFraud.Application.Abstractions;
using CapitecFraud.Application.Models;
using CapitecFraud.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CapitecFraud.Api.Endpoints;

public class FraudEndpoint:ICapitecFraudEndpoint
{
    public void RegisterEndpointRoutes(IEndpointRouteBuilder app, string prefix)
    {
        // Get fraud transactions
        app.MapPost($"{prefix}/evaluate-transactions", async (
            [FromBody]TransactionMessage transactionMessage,
            [FromServices] ITransactionQueue queue) =>
        {
            transactionMessage.Timestamp = DateTime.UtcNow;

            await queue.PublishAsync(transactionMessage);

            return ApiResults.Accepted(new
            {
                message = "Transaction queued for fraud evaluation",
                id = transactionMessage.Id,
                TransactionDate = DateTime.UtcNow
            });
        });
        
        // Get fraud transaction by identity
        app.MapGet($"{prefix}/transactions/{{id}}", async () =>
        {

        });
        
        // Get transaction audit trail
        app.MapGet($"{prefix}/audit/{{id}}", async () =>
        {

        });
    }
}