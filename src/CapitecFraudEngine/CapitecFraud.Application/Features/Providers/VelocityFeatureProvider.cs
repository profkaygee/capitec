using CapitecFraud.Application.Abstractions;
using CapitecFraud.Application.Models;

namespace CapitecFraud.Application.Features.Providers;

public class VelocityFeatureProvider(ITransactionRepository transactionRepository) 
: IFeatureProvider
{
    public async Task EnrichAsync(FraudEvaluationContext context)
    {
        var id = context.Transaction.AccountId;

        context.TransactionCountLast1Min =
            await transactionRepository.CountTransactions(id, TimeSpan.FromMinutes(1));

        context.TransactionCountLast10Min =
            await transactionRepository.CountTransactions(id, TimeSpan.FromMinutes(10));
    }
}