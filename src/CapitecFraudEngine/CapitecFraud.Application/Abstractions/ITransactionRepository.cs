using CapitecFraud.Domain.Models;

namespace CapitecFraud.Application.Abstractions;

public interface ITransactionRepository
{
    Task AddAsync(TransactionMessage transaction);
    Task<int> CountTransactions(string accountId, TimeSpan window);
    Task<TransactionMessage> GetTransaction(Guid transactionId);
}