using CapitecFraud.Domain.Abstractions.Repositories;
using CapitecFraud.Domain.Entities;
using CapitecFraud.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CapitecFraud.Infrastructure.Repositories;

public class RuleRepository : IRuleRepository
{
    private readonly CapitecFraudDbContext _db;

    public RuleRepository(CapitecFraudDbContext db)
    {
        _db = db;
    }

    public async Task<IList<RuleConfig>> GetActiveRulesAsync()
    {
        return await _db.RuleConfigs
            .Where(x => x.IsActive)
            .ToListAsync();
    }

    public async Task<FraudDecisionRuleConfig> GetDecisionConfigAsync()
    {
        return await _db.FraudDecisionRuleConfigs
            .FirstOrDefaultAsync();
    }
}