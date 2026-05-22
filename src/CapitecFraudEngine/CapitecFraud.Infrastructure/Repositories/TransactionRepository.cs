using CapitecFraud.Application.Abstractions;
using CapitecFraud.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CapitecFraud.Infrastructure.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly CapitecFraudDbContext _db;

    public TransactionRepository(CapitecFraudDbContext db)
    {
        _db = db;
    }

    public async Task<int> CountTransactions(string accountId, TimeSpan window)
    {
        var since = DateTime.UtcNow.Subtract(window);

        return await _db.Transactions
            .Where(t => t.AccountId == accountId && t.Timestamp >= since)
            .CountAsync();
    }
}