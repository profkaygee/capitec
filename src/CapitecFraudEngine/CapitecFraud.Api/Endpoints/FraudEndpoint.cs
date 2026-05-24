using CapitecFraud.Api.Common;
using CapitecFraud.Application.Abstractions;
using CapitecFraud.Application.Validation;
using CapitecFraud.Domain.Abstractions.Services;
using CapitecFraud.Domain.Enums;
using CapitecFraud.Domain.Models;
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
        app.MapGet($"{prefix}/transactions/{{transactionId}}", async (
            Guid transactionId,
            [FromServices] ITransactionRepository transactionRepository) =>
        {
            // Get the transaction from the database
            var transactionMessage = await transactionRepository.GetTransaction(transactionId);

            return transactionMessage == null 
                ? ApiResults.NotFound("Transaction not found") 
                : ApiResults.Ok(transactionMessage);
        });

        // Get transaction audit trail for last three months (default)
        app.MapGet($"{prefix}/audit/{{accountId}}", async (
            string accountId,
            DateTime? startDate,
            DateTime? endDate,
            [FromServices] IAuditService auditService) =>
        {
            if (startDate == null && endDate == null)
            {
                //Freeze the time
                 endDate = DateTime.Now;

                // Get the audit trail from the database
                startDate = endDate?.AddMonths(-3);
            }

            var auditTrail = await auditService.GetAuditLogsAsync(accountId, startDate, endDate);
            return ApiResults.Ok(auditTrail);
        });
    }

    private static FraudDecision MapGuardAction(GuardAction action)
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