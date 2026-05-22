using CapitecFraud.Domain.Entities;

namespace CapitecFraud.Domain.Abstractions.Repositories;

public interface IRuleRepository
{
    Task<IList<RuleConfig>> GetActiveRulesAsync();
    Task<FraudDecisionRuleConfig> GetDecisionConfigAsync();
}