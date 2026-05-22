using System.Text.Json;
using CapitecFraud.Application.Abstractions;
using CapitecFraud.Application.Engines;
using CapitecFraud.Application.Models;
using CapitecFraud.Application.Services;
using CapitecFraud.Domain.Abstractions.Repositories;
using CapitecFraud.Domain.Abstractions.Services;

namespace CapitecFraud.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ITransactionQueue _queue;
    private readonly IRuleRepository _ruleRepository;
    private readonly IFraudResultRepository _fraudResultRepository;
    private readonly IAuditService _auditService;
    private readonly IRealtimeNotifier _notifier;
    private readonly FraudEngine _engine;
    private readonly FeatureEnrichmentService _enrichment;

    public Worker(
        ILogger<Worker> logger,
        ITransactionQueue queue,
        IRuleRepository ruleRepository,
        IFraudResultRepository fraudResultRepository,
        IAuditService auditService,
        FraudEngine engine,
        IRealtimeNotifier notifier,
        FeatureEnrichmentService enrichment)
    {
        _logger = logger;
        _queue = queue;
        _ruleRepository = ruleRepository;
        _fraudResultRepository = fraudResultRepository;
        _auditService = auditService;
        _notifier = notifier;
        _engine = engine;
        _enrichment = enrichment;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Fraud Worker started at {Time}", DateTime.UtcNow);

        await _queue.ConsumeAsync(async (TransactionMessage message) =>
        {
            var context = await _enrichment.BuildAsync(message);
            
            using var scope = _logger.BeginScope(new Dictionary<string, object>
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
                _logger.LogInformation("Processing transaction with ID [{TransactionId}]", message.TransactionId);

                var rules = await _ruleRepository.GetActiveRulesAsync();
                _logger.LogInformation("Loaded {RuleCount} rules", rules.Count);
                
                var decisionConfig = await _ruleRepository.GetDecisionConfigAsync();
                _logger.LogInformation("Loaded decision config with config: {Config}", JsonSerializer.Serialize(decisionConfig));

                _logger.LogInformation("Engine initialized...");
                var result = _engine.Evaluate(context, rules, decisionConfig);
                
                _logger.LogInformation("Engine evaluated with result: {Result}.", JsonSerializer.Serialize(result));

                await _fraudResultRepository.AddAsync(result);
                _logger.LogInformation("Result added to repository.");
                
                await _auditService.LogAsync(message, result);
                _logger.LogInformation("Audit log added.");
                
                await _notifier.NotifyAsync(result);
                _logger.LogInformation("Notification sent.");

                _logger.LogInformation("✅ Transaction processed successfully with score {score} and decision {Decision}",
                    result.RiskScore, result.Decision);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing transaction");
            }
        });
    }
}