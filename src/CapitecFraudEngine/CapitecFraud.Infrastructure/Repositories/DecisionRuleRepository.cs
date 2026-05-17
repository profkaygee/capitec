using CapitecFraud.Domain.Rules;
using CapitecFraud.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CapitecFraud.Infrastructure.Repositories;

public class DecisionRuleRepository(CapitecFraudDbContext context)
{
    public async Task<FraudDecisionRuleConfig> GetConfig()
    {
        var config = await context.FraudDecisionRules
            .Where(x => x.IsActive)
            .FirstAsync();

        return new FraudDecisionRuleConfig
        {
            BlockThreshold = config.BlockThreshold,
            ReviewThreshold = config.ReviewThreshold
        };
    }
}