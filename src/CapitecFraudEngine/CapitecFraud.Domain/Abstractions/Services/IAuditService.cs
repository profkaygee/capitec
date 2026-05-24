using CapitecFraud.Domain.DataTransferObjects;
using CapitecFraud.Domain.Entities;
using CapitecFraud.Domain.Models;

namespace CapitecFraud.Domain.Abstractions.Services;

public interface IAuditService
{
    Task LogAsync(TransactionMessage transaction, FraudResult result);
    Task<IList<AuditLogDto>> GetAuditLogsAsync(string accountId, DateTime? fromDate, DateTime? toDate);
}