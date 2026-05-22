using CapitecFraud.Application.Abstractions;
using CapitecFraud.Application.Engines;
using CapitecFraud.Application.Models;
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

    public Worker(
        ILogger<Worker> logger,
        ITransactionQueue queue,
        IRuleRepository ruleRepository,
        IFraudResultRepository fraudResultRepository,
        IAuditService auditService,
        IRealtimeNotifier notifier)
    {
        _logger = logger;
        _queue = queue;
        _ruleRepository = ruleRepository;
        _fraudResultRepository = fraudResultRepository;
        _auditService = auditService;
        _notifier = notifier;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Fraud Worker started at {Time}", DateTime.UtcNow);

        await _queue.ConsumeAsync(async (TransactionMessage message) =>
        {
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["TransactionId"] = message.Id,
                ["AccountId"] = message.AccountId
            });

            try
            {
                _logger.LogInformation("Processing transaction with ID [{TransactionId}]", message.Id);

                var rules = await _ruleRepository.GetActiveRulesAsync();
                var decisionConfig = await _ruleRepository.GetDecisionConfigAsync();

                var engine = new FraudEngine(rules, decisionConfig);
                var result = engine.Evaluate(message);

                await _fraudResultRepository.AddAsync(result);
                await _auditService.LogAsync(message, result);
                await _notifier.NotifyAsync(result);

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