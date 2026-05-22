using CapitecFraud.Application.Abstractions;
using CapitecFraud.Application.Models;

namespace CapitecFraud.Application.Features.Providers;

public class GeoFeatureProvider : IFeatureProvider
{
    private readonly IGeoService _geo;

    public GeoFeatureProvider(IGeoService geo)
    {
        _geo = geo;
    }

    public async Task EnrichAsync(FraudEvaluationContext context)
    {
        context.GeoDistanceKmPerHour =
            await _geo.CalculateSpeed(context.Transaction.AccountId);

        context.IsVpnDetected =
            await _geo.IsVpn(context.Transaction.AccountId);
    }
}