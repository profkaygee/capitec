namespace CapitecFraud.Application.Models;

public class FraudEvaluationContext
{
    public TransactionMessage Transaction { get; set; }
    public string DeviceId { get; set; }
    public string IpAddress { get; set; }
    public int TransactionCountLast1Min { get; set; }
    public int TransactionCountLast10Min { get; set; }
    public bool IsNewDevice { get; set; }
    public int DeviceAccountCount { get; set; }
    public double GeoDistanceKmPerHour { get; set; }
    public bool IsVpnDetected { get; set; }
    public int FailedLoginAttempts { get; set; }
    public int PasswordResetLastMinutes { get; set; }
    public int DaysSinceLastTransaction { get; set; }
    public decimal SpendingDeviationPercent { get; set; }
    public bool IsNewBeneficiary { get; set; }
    public int CountryRiskScore { get; set; }
}