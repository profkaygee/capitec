using CapitecFraud.Application.Models;
using CapitecFraud.Domain.Abstractions.Rules;
using CapitecFraud.Domain.Entities;

namespace CapitecFraud.Application.Rules;

public class RuleEngine
{
    private readonly IEnumerable<RuleConfig> _rules;
    private readonly IRuleEvaluator _evaluator;

    public RuleEngine(IEnumerable<RuleConfig> rules, IRuleEvaluator evaluator)
    {
        _rules = rules;
        _evaluator = evaluator;
    }

    public int Execute(FraudEvaluationContext context)
    {
        int score = 0;

        foreach (var rule in _rules.Where(r => r.IsActive))
        {
            if (_evaluator.Evaluate(rule, context))
            {
                score += rule.ActionWeight;
            }
        }

        return score;
    }
}