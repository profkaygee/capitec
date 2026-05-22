using CapitecFraud.Api.Common;
using CapitecFraud.Application.Abstractions;
using CapitecFraud.Application.Models;
using CapitecFraud.Application.Validation;
using CapitecFraud.Domain.Entities;
using CapitecFraud.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace CapitecFraud.Api.Endpoints;

public class FraudEndpoint : ICapitecFraudEndpoint
{
    public void RegisterEndpointRoutes(IEndpointRouteBuilder app, string prefix)
    {
        // Get fraud transactions
        app.MapPost($"{prefix}/evaluate-transactions", async (
            [FromBody] TransactionMessage transactionMessage,
            [FromServices] TransactionGuard guard,
            [FromServices] ILogger<FraudEndpoint> logger,
            [FromServices] ITransactionQueue queue) =>
        {
            transactionMessage.Timestamp = DateTime.UtcNow;
            
            // Validate the message first
            var validationResult = guard.Validate(transactionMessage);
            if (!validationResult.IsValid)
            {
                logger.LogInformation("Transaction validation failed with reason: {Reason} and decision: {Decision}",
                    validationResult.Reason, MapGuardAction(validationResult.Action));
                return ApiResults.BadRequest(validationResult.Reason);
            }

            await queue.PublishAsync(transactionMessage);

            return ApiResults.Accepted(new
            {
                message = "Transaction queued for fraud evaluation",
                TransactionId = transactionMessage.TransactionId,
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

    private FraudDecision MapGuardAction(GuardAction action)
    {
        return action switch
        {
            GuardAction.Reject => FraudDecision.REJECT,
            GuardAction.Quarantine => FraudDecision.QUARANTINE,
            GuardAction.Review => FraudDecision.REVIEW,
            _ => FraudDecision.ALLOW
        };
    }
}