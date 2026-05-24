using CapitecFraud.Application.Abstractions;
using CapitecFraud.Domain.Models;

namespace CapitecFraud.Application.Services;

public class FeatureEnrichmentService(IEnumerable<IFeatureProvider> providers)
{
    public async Task<FraudEvaluationContext> BuildAsync(TransactionMessage txn)
    {
        var context = new FraudEvaluationContext
        {
            Transaction = txn
        };

        foreach (var provider in providers)
        {
            await provider.EnrichAsync(context);
        }

        return context;
    }
}