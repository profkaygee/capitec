using CapitecFraud.Application.Abstractions;
using CapitecFraud.Application.Models;
using CapitecFraud.Domain.Entities;
using CapitecFraud.Infrastructure.Persistence;
using CapitecFraud.Infrastructure.Repositories;

namespace CapitecFraud.Worker.Consumers;

public class TransactionConsumer(
    FraudEngine engine,
    RuleRepository rulesRepo,
    CapitecFraudDbContext context,
    IAuditService auditService,
    ISignalRNotifier notifier)
{
    public async Task Handle(TransactionMessage msg)
    {
        // 1. Load rules (can be cached later)
        var rules = await rulesRepo.GetActiveRules();

        // 2. Evaluate fraud
        var transaction = new Transaction
        {
            Id = msg.Id,
            AccountId = msg.AccountId,
            Amount = msg.Amount,
            Country = msg.Country,
            Category = msg.Category,
            Timestamp = msg.Timestamp
        };

        var result = engine.Evaluate(transaction, rules);

        // 3. Persist result
        context.FraudResults.Add(result);
        await context.SaveChangesAsync();

        // 4. Audit log
        await auditService.LogAsync(transaction, result);

        // 5. Real-time notify dashboard
        await notifier.NotifyAsync(result);
    }

}