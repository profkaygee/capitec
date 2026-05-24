using CapitecFraud.Application.Abstractions;
using CapitecFraud.Application.Models;
using CapitecFraud.Domain.Models;

namespace CapitecFraud.Application.Services;

public class FeatureEnrichmentService
{
    private readonly IEnumerable<IFeatureProvider> _providers;

    public FeatureEnrichmentService(IEnumerable<IFeatureProvider> providers)
    {
        _providers = providers;
    }

    public async Task<FraudEvaluationContext> BuildAsync(TransactionMessage txn)
    {
        var context = new FraudEvaluationContext
        {
            Transaction = txn
        };

        foreach (var provider in _providers)
        {
            await provider.EnrichAsync(context);
        }

        return context;
    }
}