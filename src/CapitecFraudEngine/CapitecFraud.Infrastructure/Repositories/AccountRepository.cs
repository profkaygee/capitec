using CapitecFraud.Application.Abstractions;
using CapitecFraud.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CapitecFraud.Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly CapitecFraudDbContext _db;

    public AccountRepository(CapitecFraudDbContext db)
    {
        _db = db;
    }

    public async Task<int> DaysSinceLastTransaction(string accountId)
    {
        var last = await _db.Transactions
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.Timestamp)
            .Select(t => t.Timestamp)
            .FirstOrDefaultAsync();

        return last == default ? int.MaxValue : (DateTime.UtcNow - last).Days;
    }

    public async Task<decimal> GetSpendingDeviation(string accountId)
    {
        var last30Days = DateTime.UtcNow.AddDays(-30);

        var transactions = await _db.Transactions
            .Where(t => t.AccountId == accountId && t.Timestamp >= last30Days)
            .ToListAsync();

        if (!transactions.Any()) return 0;

        var avg = transactions.Average(t => t.Amount);
        var latest = transactions.OrderByDescending(t => t.Timestamp).First().Amount;

        if (avg == 0) 
            return 0;

        var deviation = ((latest - avg) / avg) * 100;

        return Math.Abs(deviation);
    }
}