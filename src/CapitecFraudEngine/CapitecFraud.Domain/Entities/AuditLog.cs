using CapitecFraud.Domain.Enums;

namespace CapitecFraud.Domain.Entities;

public class AuditLog
{
    public long Id { get; set; }

    public Guid TransactionId { get; set; }

    public string AccountId { get; set; } = string.Empty;

    public FraudDecision Decision { get; set; }

    public int RiskScore { get; set; }

    public string Details { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Action { get; set; }
}