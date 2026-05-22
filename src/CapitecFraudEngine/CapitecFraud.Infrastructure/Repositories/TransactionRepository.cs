using CapitecFraud.Application.Abstractions;
using CapitecFraud.Application.Models;
using CapitecFraud.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CapitecFraud.Infrastructure.Repositories;

public class TransactionRepository(CapitecFraudDbContext database) 
    : ITransactionRepository
{
    public async Task AddAsync(TransactionMessage transaction)
    {
        await database.Transactions.AddAsync(transaction);
        await database.SaveChangesAsync();
    }

    public async Task<int> CountTransactions(string accountId, TimeSpan window)
    {
        var since = DateTime.UtcNow.Subtract(window);

        return await database.Transactions
            .Where(t => t.AccountId == accountId && t.Timestamp >= since)
            .CountAsync();
    }
}