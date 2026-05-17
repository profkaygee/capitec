using CapitecFraud.Application.Models;

namespace CapitecFraud.Application.Abstractions;

public interface ITransactionQueue
{
    Task PublishAsync(TransactionMessage message);
}