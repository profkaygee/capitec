namespace CapitecFraud.Application.Models;

public class TransactionMessage
{
    public Guid Id { get; set; }

    public string AccountId { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string Country { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; }

    public string Currency { get; set; } = "ZAR";

    public string MerchantId { get; set; } = string.Empty;
}