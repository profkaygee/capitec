namespace CapitecFraud.Domain.Abstractions.Services;

public interface IDeviceFingerprintService
{
    string Generate(string accountId, string ipAddress, string userAgent = "");
}