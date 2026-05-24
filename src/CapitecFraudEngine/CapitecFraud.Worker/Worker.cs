using System.Text.Json;
using CapitecFraud.Application.Abstractions;
using CapitecFraud.Application.Engines;
using CapitecFraud.Application.Models;
using CapitecFraud.Application.Services;
using CapitecFraud.Application.Validation;
using CapitecFraud.Domain.Abstractions.Repositories;
using CapitecFraud.Domain.Abstractions.Services;
using CapitecFraud.Domain.Entities;
using CapitecFraud.Domain.Enums;
using CapitecFraud.Domain.Models;

namespace CapitecFraud.Worker;

public class Worker(
    ILogger<Worker> logger,
    ITransactionQueue queue,
    IRuleRepository ruleRepository,
    IFraudResultRepository fraudResultRepository,
    IAuditService auditService,
    FraudEngine engine,
    IRealtimeNotifier notifier,
    FeatureEnrichmentService enrichment,
    ITransactionRepository transactionRepository)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Fraud Worker started at {Time}", DateTime.UtcNow);

        await queue.ConsumeAsync(async (TransactionMessage message) =>
        {
            var context = await enrichment.BuildAsync(message);

            using var scope = logger.BeginScope(new Dictionary<string, object>
            {
                ["TransactionId"] = context.Transaction.TransactionId,
                ["AccountId"] = context.Transaction.AccountId,
                ["Amount"] = context.Transaction.Amount,
                ["Country"] = context.Transaction.Country,
                ["Category"] = context.Transaction.Category,
                ["Currency"] = context.Transaction.Currency,
                ["MerchantId"] = context.Transaction.MerchantId
            });

            try
            {
                logger.LogInformation("Processing transaction with ID [{TransactionId}]", message.TransactionId);
                await transactionRepository.AddAsync(context.Transaction);

                var rules = await ruleRepository.GetActiveRulesAsync();
                logger.LogInformation("Loaded {RuleCount} rules", rules.Count);

                var decisionConfig = await ruleRepository.GetDecisionConfigAsync();
                logger.LogInformation("Loaded decision config with config: {Config}", JsonSerializer.Serialize(decisionConfig));

                logger.LogInformation("Engine initialized...");
                var result = engine.Evaluate(context, rules, decisionConfig);

                logger.LogInformation("Engine evaluated with result: {Result}.", JsonSerializer.Serialize(result));

                await fraudResultRepository.AddAsync(result);
                logger.LogInformation("Result added to repository.");

                await auditService.LogAsync(message, result);
                logger.LogInformation("Audit log added.");

                await notifier.NotifyAsync(result);
                logger.LogInformation("Notification sent.");

                logger.LogInformation("✅ Transaction processed successfully with score {score} and decision {Decision}",
                    result.RiskScore, result.Decision);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing transaction");
            }
        });
    }
}