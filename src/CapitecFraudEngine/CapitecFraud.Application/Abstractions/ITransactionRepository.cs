namespace CapitecFraud.Application.Abstractions;

public interface ITransactionRepository
{
    Task<int> CountTransactions(string accountId, TimeSpan window);
}