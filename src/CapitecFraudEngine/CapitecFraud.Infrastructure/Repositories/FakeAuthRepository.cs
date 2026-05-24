using CapitecFraud.Application.Abstractions;
using CapitecFraud.Domain.Abstractions.Services;

namespace CapitecFraud.Infrastructure.Repositories;

public class FakeAuthRepository(IClockService clockService) : IAuthRepository
{
    private readonly Dictionary<string, List<DateTime>> _failedLogins = new();
    private readonly Dictionary<string, DateTime> _passwordResets = new();

    public Task<int> GetFailedLogins(string accountId, TimeSpan window)
    {
        if (!_failedLogins.TryGetValue(accountId, out var value))
            return Task.FromResult(0);

        var since = clockService.UtcNow.Subtract(window);

        var count = value.Count(x => x >= since);

        return Task.FromResult(count);
    }

    public Task<int> MinutesSincePasswordReset(string accountId)
    {
        if (!_passwordResets.TryGetValue(accountId, out var passwordResetDateTime))
            return Task.FromResult(int.MaxValue);

        var minutes = (clockService.UtcNow - passwordResetDateTime).Minutes;
        return Task.FromResult(minutes);
    }

    public void SimulateFailedLogin(string accountId)
    {
        if (!_failedLogins.ContainsKey(accountId))
            _failedLogins[accountId] = new List<DateTime>();

        _failedLogins[accountId].Add(clockService.UtcNow);
    }

    public void SimulatePasswordReset(string accountId)
    {
        _passwordResets[accountId] = clockService.UtcNow;
    }
}