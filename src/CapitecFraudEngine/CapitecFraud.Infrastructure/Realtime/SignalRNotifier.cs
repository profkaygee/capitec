using CapitecFraud.Application.Abstractions;
using CapitecFraud.Domain.Entities;
using Microsoft.AspNetCore.SignalR;

namespace CapitecFraud.Infrastructure.Realtime;

public class SignalRNotifier(IHubContext<Hub> hub):ISignalRNotifier
{
    public async Task NotifyAsync(FraudResult result)
    {
        await hub.Clients.All.SendAsync(
            "FraudEvaluated",
            new
            {
                result.TransactionId,
                result.RiskScore,
                result.Decision,
                Flags = result.Flags
            });
    }

    public async Task NotifyTransactionAsync(long transactionId, string message)
    {
        await hub.Clients.All.SendAsync(
            "TransactionUpdate",
            new
            {
                transactionId,
                message
            });
    }
}