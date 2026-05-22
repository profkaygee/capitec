using System.Reflection;
using CapitecFraud.Application.Models;
using CapitecFraud.Domain.Entities;
using CapitecFraud.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace CapitecFraud.Application.Engines;

public class FraudEngine
{
    public readonly ILogger<FraudEngine> _logger;

    public FraudEngine(ILogger<FraudEngine> logger)
    {
        _logger = logger;
    }

    public FraudResult Evaluate(FraudEvaluationContext fraudEvaluationContext, IList<RuleConfig> rules,FraudDecisionRuleConfig _decisionConfig)
    {
        int score = 0;
        var flags = new List<FraudFlag>();

        foreach (var rule in rules)
        {
            if (!rule.IsActive)
            {
                _logger.LogInformation("Current rule is inactive: {RuleName}", rule.Name);
                continue;
            }

            var fieldValue = GetField(fraudEvaluationContext, rule.Field);
            _logger.LogInformation("Evaluating rule: {RuleName} for field: {Field} with value: {FieldValue}", rule.Name, rule.Field, fieldValue);

            if (Compare(fieldValue, rule))
            {
                score += rule.ActionWeight;
                _logger.LogInformation("Rule {RuleName} triggered for field: {Field} with value: {FieldValue} and score: {Score}", rule.Name, rule.Field, fieldValue, score);

                flags.Add(new FraudFlag
                {
                    RuleName = rule.Name,
                    Field = rule.Field,
                    Operator = rule.Operator,
                    TriggerValue = rule.Value,
                    ScoreImpact = rule.ActionWeight
                });
                _logger.LogInformation("Added flag: {Flag}", flags.Last());
            }
        }

        return new FraudResult
        {
            TransactionId = fraudEvaluationContext.Transaction.TransactionId,
            RiskScore = score,
            Decision = Decide(score, _decisionConfig),
            Flags = flags
        };
    }

    private object GetField(FraudEvaluationContext fraudEvaluationContext, string field)
    {
        _logger.LogInformation("Getting field: {Field} from context", field);
        
        return field switch
        {
            "Id" => fraudEvaluationContext.Transaction.Id,
            "TransactionId" => fraudEvaluationContext.Transaction.TransactionId,
            "AccountId" => fraudEvaluationContext.Transaction.AccountId,
            "Amount" => fraudEvaluationContext.Transaction.Amount,
            "Country" => fraudEvaluationContext.Transaction.Country,
            "Category" => fraudEvaluationContext.Transaction.Category,
            "Timestamp" => fraudEvaluationContext.Transaction.Timestamp,
            "Currency" => fraudEvaluationContext.Transaction.Currency,
            "MerchantId" => fraudEvaluationContext.Transaction.MerchantId,
            "DeviceId" => fraudEvaluationContext.DeviceId,
            "IpAddress" => fraudEvaluationContext.IpAddress,
            "TransactionCountLast1Min" => fraudEvaluationContext.TransactionCountLast1Min,
            "TransactionCountLast10Min" => fraudEvaluationContext.TransactionCountLast10Min,
            "IsNewDevice" => fraudEvaluationContext.IsNewDevice,
            "DeviceAccountCount" => fraudEvaluationContext.DeviceAccountCount,
            "GeoDistanceKmPerHour" => fraudEvaluationContext.GeoDistanceKmPerHour,
            "IsVpnDetected" => fraudEvaluationContext.IsVpnDetected,
            "FailedLoginAttempts" => fraudEvaluationContext.FailedLoginAttempts,
            "PasswordResetLastMinutes" => fraudEvaluationContext.PasswordResetLastMinutes,
            "DaysSinceLastTransaction" => fraudEvaluationContext.DaysSinceLastTransaction,
            "SpendingDeviationPercent" => fraudEvaluationContext.SpendingDeviationPercent,
            "IsNewBeneficiary" => fraudEvaluationContext.IsNewBeneficiary,
            "CountryRiskScore" => fraudEvaluationContext.CountryRiskScore,
            _ => null
        };
    }

    private bool Compare(object fieldValue, RuleConfig rule)
    {
            return false;

        return rule.Operator switch
        {
            ">" => Convert.ToDecimal(fieldValue) > Convert.ToDecimal(rule.Value),
            "<" => Convert.ToDecimal(fieldValue) < Convert.ToDecimal(rule.Value),
            "=" => fieldValue.ToString() == rule.Value,
            "IN" => rule.Value.Split(',').Contains(fieldValue.ToString()),
            _ => false
        };
    }

    public FraudDecision Decide(int score,FraudDecisionRuleConfig _decisionConfig)
    {
        if (score >= _decisionConfig.BlockThreshold)
            return FraudDecision.Block;

        return score >= _decisionConfig.ReviewThreshold 
            ? FraudDecision.Review 
            : FraudDecision.Allow;
    }
}