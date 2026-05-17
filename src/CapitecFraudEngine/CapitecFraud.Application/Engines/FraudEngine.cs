using CapitecFraud.Domain.Entities;
using CapitecFraud.Domain.Rules;

public class FraudEngine
{
    private readonly FraudDecisionRuleConfig _decisionConfig;

    public FraudEngine(FraudDecisionRuleConfig decisionConfig)
    {
        _decisionConfig = decisionConfig;
    }

    public FraudResult Evaluate(Transaction tx, IEnumerable<RuleConfig> rules)
    {
        var flags = new List<FraudFlag>();
        int score = 0;

        foreach (var rule in rules.Where(r => r.IsActive))
        {
            var value = GetFieldValue(tx, rule.Field);

            if (value == null)
                continue;

            if (EvaluateRule(rule, value))
            {
                flags.Add(new FraudFlag
                {
                    RuleName = rule.Name,
                    Description = $"{rule.Field} {rule.Operator} {rule.Value}",
                    Field = rule.Field,
                    TriggerValue = rule.Value,
                    Operator = rule.Operator,
                    ScoreImpact = rule.ActionWeight
                });

                score += rule.ActionWeight;
            }
        }

        return new FraudResult
        {
            TransactionId = tx.Id,
            RiskScore = score,
            Decision = Decide(score),
            Flags = flags
        };
    }

    private bool EvaluateRule(RuleConfig rule, object value)
    {
        return rule.Operator switch
        {
            ">" => Convert.ToDecimal(value) > Convert.ToDecimal(rule.Value),
            "<" => Convert.ToDecimal(value) < Convert.ToDecimal(rule.Value),
            "=" => value.ToString() == rule.Value,
            "IN" => rule.Value.Split(',').Contains(value.ToString()),
            _ => false
        };
    }

    private object? GetFieldValue(Transaction txn, string field)
    {
        return Map.TryGetValue(field, out var getter)
            ? getter(txn)
            : null;
    }

    private static readonly Dictionary<string, Func<Transaction, object>> Map = new()
    {
        ["Amount"] = t => t.Amount,
        ["Country"] = t => t.Country,
        ["Category"] = t => t.Category,
        ["AccountId"] = t => t.AccountId
    };

    private FraudDecision Decide(int score)
    {
        if (score >= _decisionConfig.BlockThreshold)
            return FraudDecision.Block;

        return score >= _decisionConfig.ReviewThreshold 
            ? FraudDecision.Review 
            : FraudDecision.Allow;
    }
}