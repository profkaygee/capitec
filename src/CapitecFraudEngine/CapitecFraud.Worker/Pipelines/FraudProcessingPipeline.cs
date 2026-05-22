using CapitecFraud.Application.Engines;
using CapitecFraud.Application.Models;
using CapitecFraud.Application.Rules;
using CapitecFraud.Application.Services;
using CapitecFraud.Domain.Entities;

public class FraudProcessingPipeline
{
    private readonly FeatureEnrichmentService _enrichment;
    private readonly RuleEngine _ruleEngine;
    private readonly FraudEngine _decision;

    public FraudProcessingPipeline(
        FeatureEnrichmentService enrichment,
        RuleEngine ruleEngine,
        FraudEngine decision)
    {
        _enrichment = enrichment;
        _ruleEngine = ruleEngine;
        _decision = decision;
    }

    public async Task<FraudResult> ProcessAsync(TransactionMessage transaction)
    {
        // 1. Enrich
        var context = await _enrichment.BuildAsync(transaction);

        // 2. Score
        var score = _ruleEngine.Execute(context);

        // 3. Decide
        var decision = _decision.Decide(score);

        return new FraudResult
        {
            Id = transaction.Id,
            TransactionId = transaction.TransactionId,
            RiskScore = score,
            Decision = decision
        };
    }
}