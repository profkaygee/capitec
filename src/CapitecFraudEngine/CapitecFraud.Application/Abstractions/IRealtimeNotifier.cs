using CapitecFraud.Domain.Entities;

namespace CapitecFraud.Application.Abstractions;

public interface IRealtimeNotifier
{
    Task NotifyAsync(FraudResult result);
}