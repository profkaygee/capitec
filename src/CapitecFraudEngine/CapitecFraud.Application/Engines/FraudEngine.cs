using CapitecFraud.Application.Models;
using CapitecFraud.Domain.Entities;
using CapitecFraud.Domain.Enums;

namespace CapitecFraud.Application.Engines;

public class FraudEngine
{
    private readonly IEnumerable<RuleConfig> _rules;
    private readonly FraudDecisionRuleConfig _decisionConfig;

    public FraudEngine(IEnumerable<RuleConfig> rules,
        FraudDecisionRuleConfig decisionConfig)
    {
        _rules = rules;
        _decisionConfig = decisionConfig;
    }

    public FraudResult Evaluate(TransactionMessage tx)
    {
        int score = 0;
        var flags = new List<FraudFlag>();

        foreach (var rule in _rules)
        {
            if (!rule.IsActive) 
                continue;

            var fieldValue = GetField(tx, rule.Field);

            if (Compare(fieldValue, rule))
            {
                score += rule.ActionWeight;

                flags.Add(new FraudFlag
                {
                    RuleName = rule.Name,
                    Field = rule.Field,
                    Operator = rule.Operator,
                    TriggerValue = rule.Value,
                    ScoreImpact = rule.ActionWeight
                });
            }
        }

        return new FraudResult
        {
            RiskScore = score,
            Decision = Decide(score),
            Flags = flags
        };
    }

    private object GetField(TransactionMessage tx, string field)
    {
        return field switch
        {
            "Amount" => tx.Amount,
            "Country" => tx.Country,
            "Category" => tx.Category,
            "AccountId" => tx.AccountId,
            "Currency" => tx.Currency,
            _ => null
        };
    }

    private bool Compare(object fieldValue, RuleConfig rule)
    {
        if (fieldValue == null) return false;

        return rule.Operator switch
        {
            ">" => Convert.ToDecimal(fieldValue) > Convert.ToDecimal(rule.Value),
            "<" => Convert.ToDecimal(fieldValue) < Convert.ToDecimal(rule.Value),
            "=" => fieldValue.ToString() == rule.Value,
            "IN" => rule.Value.Split(',').Contains(fieldValue.ToString()),
            _ => false
        };
    }

    public FraudDecision Decide(int score)
    {
        if (score >= _decisionConfig.BlockThreshold)
            return FraudDecision.Block;

        return score >= _decisionConfig.ReviewThreshold 
            ? FraudDecision.Review 
            : FraudDecision.Allow;
    }
}