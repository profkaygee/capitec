namespace CapitecFraud.Domain.Entities;

public class FraudDecisionRuleConfig
{
    public long Id { get; set; }
    
    public string Name { get; set; }

    public int ReviewThreshold { get; set; }

    public int BlockThreshold { get; set; }
}