using CapitecFraud.Application.Models;

namespace CapitecFraud.Application.Abstractions;

public interface IFeatureProvider
{
    Task EnrichAsync(FraudEvaluationContext context);
}