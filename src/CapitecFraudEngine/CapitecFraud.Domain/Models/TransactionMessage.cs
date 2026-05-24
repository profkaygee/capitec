namespace CapitecFraud.Application.Models;

public class TransactionMessage
{
    public long Id { get; set; }
    public Guid TransactionId { get; set; } = Guid.NewGuid();
    public string AccountId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Country { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Currency { get; set; } = "ZAR";
    public string MerchantId { get; set; } = string.Empty;
}