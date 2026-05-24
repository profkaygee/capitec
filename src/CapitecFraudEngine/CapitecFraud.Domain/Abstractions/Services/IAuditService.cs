using CapitecFraud.Application.Models;
using CapitecFraud.Domain.Entities;
using CapitecFraud.Domain.Models;

namespace CapitecFraud.Domain.Abstractions.Services;

public interface IAuditService
{
    Task LogAsync(TransactionMessage transaction, FraudResult result);
}