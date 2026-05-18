using CapitecFraud.Domain.Entities;

namespace CapitecFraud.Application.Abstractions;

public interface IRuleRepository
{
    Task<IList<RuleConfig>> GetActiveRules();

    Task<FraudDecisionRuleConfig> GetDecisionConfig();
}