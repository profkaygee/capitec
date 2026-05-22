using CapitecFraud.Domain.Enums;

namespace CapitecFraud.Domain.Entities;

public class FraudResult
{
    public long Id { get; set; }

    public Guid TransactionId { get; set; }

    public int RiskScore { get; set; }

    public FraudDecision Decision { get; set; }
    
    public string Reason { get; set; } = string.Empty;

    public IList<FraudFlag> Flags { get; set; } = new List<FraudFlag>();
}