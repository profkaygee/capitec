using CapitecFraud.Application.Abstractions;
using CapitecFraud.Application.Models;

namespace CapitecFraud.Application.Services;

public class FeatureEnrichmentService
{
    private readonly IEnumerable<IFeatureProvider> _providers;

    public FeatureEnrichmentService(IEnumerable<IFeatureProvider> providers)
    {
        _providers = providers;
    }

    public async Task<FraudEvaluationContext> BuildAsync(TransactionMessage tx)
    {
        var context = new FraudEvaluationContext
        {
            Transaction = tx
        };

        foreach (var provider in _providers)
        {
            await provider.EnrichAsync(context);
        }

        return context;
    }
}