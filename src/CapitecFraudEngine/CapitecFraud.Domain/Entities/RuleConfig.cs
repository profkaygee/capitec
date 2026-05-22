namespace CapitecFraud.Domain.Entities;

public class RuleConfig
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Field { get; set; } = string.Empty;
    public string Operator { get; set; } = string.Empty; // > < = IN
    public string Value { get; set; } = string.Empty;
    public int ActionWeight { get; set; }
    public bool IsActive { get; set; } = true;
}