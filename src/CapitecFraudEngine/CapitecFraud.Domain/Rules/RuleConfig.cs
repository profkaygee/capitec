namespace CapitecFraud.Domain.Rules;

public class RuleConfig
{
    public string Name { get; set; }
    public string Field { get; set; }
    public string Operator { get; set; }
    public string Value { get; set; }
    public int ActionWeight { get; set; }
    public bool IsActive { get; set; }
}