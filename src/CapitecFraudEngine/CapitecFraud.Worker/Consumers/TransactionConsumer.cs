using CapitecFraud.Application.Abstractions;
using CapitecFraud.Application.Engines;
using CapitecFraud.Application.Models;
using CapitecFraud.Domain.Entities;
using CapitecFraud.Infrastructure.Persistence;

namespace CapitecFraud.Worker.Consumers;

public class TransactionConsumer
{
    private readonly IServiceProvider _provider;

    public TransactionConsumer(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task Handle(TransactionMessage msg)
    {
        using var scope = _provider.CreateScope();

        var rulesRepo = scope.ServiceProvider.GetRequiredService<IRuleRepository>();
        var resultRepo = scope.ServiceProvider.GetRequiredService<IFraudResultRepository>();
        var audit = scope.ServiceProvider.GetRequiredService<IAuditService>();
        var notifier = scope.ServiceProvider.GetRequiredService<IRealtimeNotifier>();

        var rules = await rulesRepo.GetActiveRules();
        var decisionConfig = await rulesRepo.GetDecisionConfig();

        var engine = new FraudEngine(rules, decisionConfig);

        var result = engine.Evaluate(msg);

        await resultRepo.SaveAsync(result);
        
        await audit.LogAsync(new Transaction()
        {
            Id = msg.Id,
            AccountId = msg.AccountId,
            Amount = msg.Amount,
            Country = msg.Country,
            Category = msg.Category,
            Currency = msg.Currency,
            MerchantId = msg.MerchantId,
            Timestamp = msg.Timestamp
        }, result);

        await notifier.NotifyAsync(result);
    }
}