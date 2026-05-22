using CapitecFraud.Application.Abstractions;
using CapitecFraud.Application.Models;

namespace CapitecFraud.Application.Features.Providers;

public class AccountBehaviorFeatureProvider(IAccountRepository repo)
    : IFeatureProvider
{
    public async Task EnrichAsync(FraudEvaluationContext context)
    {
        var id = context.Transaction.AccountId;

        context.DaysSinceLastTransaction =
            await repo.DaysSinceLastTransaction(id);

        context.SpendingDeviationPercent =
            await repo.GetSpendingDeviation(id);
    }
}