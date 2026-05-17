namespace CapitecFraud.Domain.Entities;

public class FraudDecisionRuleEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int BlockThreshold { get; set; }

    public int ReviewThreshold { get; set; }

    public bool IsActive { get; set; }
}