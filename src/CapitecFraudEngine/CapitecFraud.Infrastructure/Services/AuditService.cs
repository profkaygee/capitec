using System.Text.Json;
using CapitecFraud.Application.Abstractions;
using CapitecFraud.Domain.Entities;
using CapitecFraud.Domain.Enums;
using CapitecFraud.Infrastructure.Persistence;

namespace CapitecFraud.Infrastructure.Services;

public class AuditService(CapitecFraudDbContext context):IAuditService
{
    public async Task LogAsync(Transaction transaction, FraudResult result)
    {
        var audit = new AuditLog
        {
            TransactionId = transaction.Id,
            AccountId = transaction.AccountId,
            Action = "FRAUD_EVALUATION",
            Decision = result.Decision,
            RiskScore = result.RiskScore,
            Details = JsonSerializer.Serialize(result),
            CreatedAt = DateTime.UtcNow
        };

        context.AuditLogs.Add(audit);
        await context.SaveChangesAsync();
    }

    public async Task LogEventAsync(string action, string details, Guid? transactionId = null)
    {
        var audit = new AuditLog
        {
            TransactionId = transactionId ?? Guid.Empty,
            AccountId = "",
            Action = action,
            Decision = FraudDecision.Review, // We review by default
            RiskScore = 0,
            Details = details,
            CreatedAt = DateTime.UtcNow
        };

        context.AuditLogs.Add(audit);
        await context.SaveChangesAsync();
    }
}