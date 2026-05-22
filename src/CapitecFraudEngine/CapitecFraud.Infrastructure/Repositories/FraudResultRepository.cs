using CapitecFraud.Domain.Abstractions.Repositories;
using CapitecFraud.Domain.Entities;
using CapitecFraud.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CapitecFraud.Infrastructure.Repositories;

public class FraudResultRepository : IFraudResultRepository
{
    private readonly CapitecFraudDbContext _database;

    public FraudResultRepository(CapitecFraudDbContext db)
    {
        _database = db;
    }

    public async Task AddAsync(FraudResult result)
    {
        await _database.FraudResults.AddAsync(result);
        await _database.SaveChangesAsync();
    }
}