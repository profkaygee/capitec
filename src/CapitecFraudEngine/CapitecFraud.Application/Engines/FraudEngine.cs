using CapitecFraud.Domain.Entities;
using CapitecFraud.Domain.Enums;
using CapitecFraud.Domain.Models;
using Microsoft.Extensions.Logging;

namespace CapitecFraud.Application.Engines;

public class FraudEngine(ILogger<FraudEngine> logger)
{
    public FraudResult Evaluate(FraudEvaluationContext fraudEvaluationContext, IList<RuleConfig> rules,FraudDecisionRuleConfig decisionConfig)
    {
        int score = 0;
        var flags = new List<FraudFlag>();

        foreach (var rule in rules)
        {
            if (!rule.IsActive)
            {
                continue;
            }

            var fieldValue = GetField(fraudEvaluationContext, rule.Field);

            if (!Compare(fieldValue, rule)) 
                continue;
            
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

        return new FraudResult
        {
            TransactionId = fraudEvaluationContext.Transaction.TransactionId,
            RiskScore = score,
            Decision = Decide(score, decisionConfig),
            Flags = flags
        };
    }

    private object GetField(FraudEvaluationContext fraudEvaluationContext, string field)
    {
        logger.LogInformation("Getting field: {Field} from context", field);
        
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

    private static bool Compare(object fieldValue, RuleConfig rule)
    {
        if (fieldValue == null) 
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

    public static FraudDecision Decide(int score,FraudDecisionRuleConfig decisionConfig)
    {
        if (score >= decisionConfig.BlockThreshold)
            return FraudDecision.BLOCK;

        return score >= decisionConfig.ReviewThreshold 
            ? FraudDecision.REVIEW 
            : FraudDecision.ALLOW;
    }
}