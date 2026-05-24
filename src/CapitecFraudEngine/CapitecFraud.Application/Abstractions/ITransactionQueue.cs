using CapitecFraud.Domain.Models;

namespace CapitecFraud.Application.Abstractions;

public interface ITransactionQueue
{
    Task ConsumeAsync(Func<TransactionMessage, Task> handler);
    Task<bool> PublishAsync(TransactionMessage message);
}