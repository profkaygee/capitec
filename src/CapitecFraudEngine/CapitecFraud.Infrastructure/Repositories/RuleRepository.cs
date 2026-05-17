using CapitecFraud.Domain.Rules;
using CapitecFraud.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CapitecFraud.Infrastructure.Repositories;

public class RuleRepository(CapitecFraudDbContext context)
{
    public async Task<List<RuleConfig>> GetActiveRules()
    {
        return await context.FraudRules
            .Where(x => x.IsActive)
            .Select(x => new RuleConfig
            {
                Name = x.Name,
                Field = x.Field,
                Operator = x.Operator,
                Value = x.Value,
                ActionWeight = x.ActionWeight,
                IsActive = x.IsActive
            })
            .ToListAsync();
    }
}