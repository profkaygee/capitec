namespace CapitecFraud.Domain.Rules;

public class FraudDecisionRuleConfig
{
    public int BlockThreshold { get; set; }

    public int ReviewThreshold { get; set; }
}