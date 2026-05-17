namespace CapitecFraud.Domain.Entities;

public class FraudFlag
{
    public long Id { get; set; }
    public long FraudResultId { get; set; }
    public string RuleName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Field { get; set; } = string.Empty;
    public string TriggerValue { get; set; } = string.Empty;
    public string Operator { get; set; } = string.Empty;
    public int ScoreImpact { get; set; }
}