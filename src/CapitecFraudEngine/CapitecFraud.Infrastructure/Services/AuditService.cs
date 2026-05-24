using System.Text.Json;
using CapitecFraud.Application.Models;
using CapitecFraud.Domain.Abstractions.Services;
using CapitecFraud.Domain.Entities;
using CapitecFraud.Domain.Models;
using CapitecFraud.Infrastructure.Persistence;

namespace CapitecFraud.Infrastructure.Services;

public class AuditService(CapitecFraudDbContext context):IAuditService
{
    public async Task LogAsync(TransactionMessage transaction, FraudResult result)
    {
        var audit = new AuditLog
        {
            TransactionId = transaction.TransactionId,
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
}