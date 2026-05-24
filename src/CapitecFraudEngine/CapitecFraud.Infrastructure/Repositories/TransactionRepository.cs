using CapitecFraud.Application.Abstractions;
using CapitecFraud.Domain.Abstractions.Services;
using CapitecFraud.Domain.Models;
using CapitecFraud.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CapitecFraud.Infrastructure.Repositories;

public class TransactionRepository(CapitecFraudDbContext database,
    IClockService clock) : ITransactionRepository
{
    public async Task AddAsync(TransactionMessage transaction)
    {
        await database.Transactions.AddAsync(transaction);
        await database.SaveChangesAsync();
    }

    public async Task<int> CountTransactions(string accountId, TimeSpan window)
    {
        DateTime since;

        if (clock.UtcNow == DateTime.MinValue)
            since = DateTime.Now.Subtract(window);
        else
            since = clock.UtcNow.Subtract(window);

        return await database.Transactions
            .Where(t => t.AccountId == accountId && t.Timestamp >= since)
            .CountAsync();
    }

    public async Task<TransactionMessage> GetTransaction(Guid transactionId)
    {
        return await database.Transactions.FirstOrDefaultAsync(x => x.TransactionId == transactionId);
    }
}