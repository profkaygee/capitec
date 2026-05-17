using CapitecFraud.Domain.Entities;

namespace CapitecFraud.Application.Abstractions;

public interface IAuditService
{
    Task LogAsync(Transaction transaction, FraudResult result);
    Task LogEventAsync(string action, string details, Guid? transactionId = null);
}