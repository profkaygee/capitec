using CapitecFraud.Domain.Entities;

namespace CapitecFraud.Application.Abstractions;

public interface ISignalRNotifier
{
    Task NotifyAsync(FraudResult result);

    Task NotifyTransactionAsync(long transactionId, string message);
}