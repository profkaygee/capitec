using CapitecFraud.Domain.Enums;

namespace CapitecFraud.Domain.DataTransferObjects;

public class AuditLogDto
{
    public long Id { get; set; }
    public Guid TransactionId { get; set; }
    public string AccountId { get; set; }
    public FraudDecision Decision { get; set; }
    public decimal RiskScore { get; set; }
    public string Details { get; set; }
}