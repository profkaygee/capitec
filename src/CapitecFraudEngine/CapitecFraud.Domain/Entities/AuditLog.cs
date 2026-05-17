namespace CapitecFraud.Domain.Entities;

public class AuditLog
{
    public long Id { get; set; }
    public Guid TransactionId { get; set; }
    public string AccountId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Decision { get; set; } = string.Empty;
    public int RiskScore { get; set; }
    public string Details { get; set; } = string.Empty; // JSON payload of FraudResult (important for traceability)Ï
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}