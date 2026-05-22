namespace CapitecFraud.Application.Abstractions;

public interface IGeoService
{
    Task<double> CalculateSpeed(string accountId);
    Task<bool> IsVpn(string accountId);
}