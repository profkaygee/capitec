namespace CapitecFraud.Application.Abstractions;

public interface IAccountRepository
{
    Task<int> DaysSinceLastTransaction(string accountId);
    Task<decimal> GetSpendingDeviation(string accountId);
}