using CapitecFraud.Application.Abstractions;
using CapitecFraud.Application.Models;

namespace CapitecFraud.Application.Features.Providers;

public class VelocityFeatureProvider : IFeatureProvider
{
    private readonly ITransactionRepository _repo;

    public VelocityFeatureProvider(ITransactionRepository repo)
    {
        _repo = repo;
    }

    public async Task EnrichAsync(FraudEvaluationContext context)
    {
        var id = context.Transaction.AccountId;

        context.TransactionCountLast1Min =
            await _repo.CountTransactions(id, TimeSpan.FromMinutes(1));

        context.TransactionCountLast10Min =
            await _repo.CountTransactions(id, TimeSpan.FromMinutes(10));
    }
}