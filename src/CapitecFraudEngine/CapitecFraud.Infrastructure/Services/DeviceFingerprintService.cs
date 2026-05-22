using System.Security.Cryptography;
using System.Text;

namespace CapitecFraud.Infrastructure.Services;

public class DeviceFingerprintService
{
    public string Generate(string accountId, string ipAddress, string userAgent = "")
    {
        var raw = $"{accountId}-{ipAddress}-{userAgent}";

        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(raw));

        return Convert.ToBase64String(bytes);
    }
}