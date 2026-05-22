using CapitecFraud.Domain.Abstractions.Repositories;
using CapitecFraud.Domain.Entities;
using CapitecFraud.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CapitecFraud.Infrastructure.Repositories;

public class RuleRepository(CapitecFraudDbContext database) 
    : IRuleRepository
{
    public async Task<IList<RuleConfig>> GetActiveRulesAsync()
    {
        return await database.RuleConfigs
            .Where(x => x.IsActive)
            .ToListAsync();
    }

    public async Task<FraudDecisionRuleConfig> GetDecisionConfigAsync()
    {
        return await database.FraudDecisionRuleConfigs
            .FirstOrDefaultAsync();
    }
}