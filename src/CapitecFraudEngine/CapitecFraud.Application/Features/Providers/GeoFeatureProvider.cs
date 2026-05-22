using CapitecFraud.Application.Abstractions;
using CapitecFraud.Application.Models;

namespace CapitecFraud.Application.Features.Providers;

public class GeoFeatureProvider(IGeoService geoService) : IFeatureProvider
{
    public async Task EnrichAsync(FraudEvaluationContext context)
    {
        context.GeoDistanceKmPerHour =
            await geoService.CalculateSpeed(context.Transaction.AccountId);

        context.IsVpnDetected =
            await geoService.IsVpn(context.Transaction.AccountId);
    }
}