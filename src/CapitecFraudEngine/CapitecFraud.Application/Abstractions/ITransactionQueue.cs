using CapitecFraud.Application.Models;

namespace CapitecFraud.Application.Abstractions;

public interface ITransactionQueue
{
    Task ConsumeAsync(Func<TransactionMessage, Task> handler);
    Task PublishAsync(TransactionMessage message);
}