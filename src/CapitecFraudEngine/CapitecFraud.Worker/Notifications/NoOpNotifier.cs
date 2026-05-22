using CapitecFraud.Application.Abstractions;
using CapitecFraud.Domain.Entities;

namespace CapitecFraud.Worker.Notifications;

public class NoOpNotifier:IRealtimeNotifier
{
    public Task NotifyAsync(FraudResult result)
    {
        return Task.CompletedTask;
    }
}