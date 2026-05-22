using CapitecFraud.Application.Models;
using CapitecFraud.Domain.Entities;

namespace CapitecFraud.Domain.Abstractions.Services;

public interface IAuditService
{
    Task LogAsync(TransactionMessage transaction, FraudResult result);
    Task LogEventAsync(string action, string details, Guid? transactionId = null);
}