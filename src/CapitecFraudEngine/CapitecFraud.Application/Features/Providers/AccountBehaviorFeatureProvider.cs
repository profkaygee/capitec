using CapitecFraud.Application.Abstractions;
using CapitecFraud.Application.Models;

namespace CapitecFraud.Application.Features.Providers;

public class AccountBehaviorFeatureProvider:IFeatureProvider
{
    private readonly IAccountRepository _repo;

    public AccountBehaviorFeatureProvider(IAccountRepository repo)
    {
        _repo = repo;
    }

    public async Task EnrichAsync(FraudEvaluationContext context)
    {
        var id = context.Transaction.AccountId;

        context.DaysSinceLastTransaction =
            await _repo.DaysSinceLastTransaction(id);

        context.SpendingDeviationPercent =
            await _repo.GetSpendingDeviation(id);
    }
}