using System.Text.Json;
using CapitecFraud.Domain.Abstractions.Services;
using CapitecFraud.Domain.DataTransferObjects;
using CapitecFraud.Domain.Entities;
using CapitecFraud.Domain.Models;
using CapitecFraud.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

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

    public async Task<IList<AuditLogDto>> GetAuditLogsAsync(string accountId, DateTime? fromDate, DateTime? toDate)
    {
        var auditTrails = await context.AuditLogs
            .Where(x => x.AccountId == accountId
                && x.CreatedAt >= fromDate
                && x.CreatedAt <= toDate)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new AuditLogDto
            {
                Id = x.Id,
                TransactionId = x.TransactionId,
                AccountId = x.AccountId,
                Decision = x.Decision,
                RiskScore = x.RiskScore,
                Details = x.Details
            }).ToListAsync();
        
        return auditTrails;
    }
}