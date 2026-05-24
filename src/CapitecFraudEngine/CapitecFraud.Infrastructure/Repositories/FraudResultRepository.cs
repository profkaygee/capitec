using CapitecFraud.Domain.Abstractions.Repositories;
using CapitecFraud.Domain.Entities;
using CapitecFraud.Infrastructure.Persistence;

namespace CapitecFraud.Infrastructure.Repositories;

public class FraudResultRepository(CapitecFraudDbContext database)
    : IFraudResultRepository
{
    public async Task AddAsync(FraudResult result)
    {
        await database.FraudResults.AddAsync(result);
        await database.SaveChangesAsync();
    }
}