using CapitecFraud.Application.Models;
using CapitecFraud.Domain.Entities;

namespace CapitecFraud.Domain.Abstractions.Rules;

public interface IRuleEvaluator
{
    bool Evaluate(RuleConfig rule, FraudEvaluationContext context);
}