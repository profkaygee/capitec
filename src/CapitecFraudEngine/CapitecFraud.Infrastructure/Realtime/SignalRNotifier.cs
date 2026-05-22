using CapitecFraud.Application.Abstractions;
using CapitecFraud.Domain.Entities;
using Microsoft.AspNetCore.SignalR;

namespace CapitecFraud.Infrastructure.Realtime;

public class SignalRNotifier : IRealtimeNotifier
{
    private readonly IHubContext<FraudNotificationHub> _hub;

    public SignalRNotifier(IHubContext<FraudNotificationHub> hub)
    {
        _hub = hub;
    }

    public Task NotifyAsync(FraudResult result)
    {
        return _hub.Clients.All.SendAsync("fraudDetected", result);
    }
}