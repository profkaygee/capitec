using CapitecFraud.Application.Abstractions;

namespace CapitecFraud.Infrastructure.Services;

public class GeoService : IGeoService
{
    private static readonly Random _rand = new();

    public Task<double> CalculateSpeed(string accountId)
    {
        // Simulate geo velocity (km/h)
        var speed = _rand.Next(10, 950);
        return Task.FromResult((double)speed);
    }

    public Task<bool> IsVpn(string accountId)
    {
        var isVpn = _rand.Next(0, 50) > 25;
        return Task.FromResult(isVpn);
    }
}