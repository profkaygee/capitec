using CapitecFraud.Application.Abstractions;

namespace CapitecFraud.Infrastructure.Repositories;

public class FakeAuthRepository : IAuthRepository
{
    private static readonly Dictionary<string, List<DateTime>> FailedLogins = new();
    private static readonly Dictionary<string, DateTime> PasswordResets = new();

    public Task<int> GetFailedLogins(string accountId, TimeSpan window)
    {
        if (!FailedLogins.ContainsKey(accountId))
            return Task.FromResult(0);

        var since = DateTime.UtcNow.Subtract(window);

        var count = FailedLogins[accountId]
            .Count(x => x >= since);

        return Task.FromResult(count);
    }

    public Task<int> MinutesSincePasswordReset(string accountId)
    {
        if (!PasswordResets.TryGetValue(accountId, out var passwordResetDateTime))
            return Task.FromResult(int.MaxValue);

        var minutes = (DateTime.UtcNow - passwordResetDateTime).Minutes;
        return Task.FromResult(minutes);
    }

    public static void SimulateFailedLogin(string accountId)
    {
        if (!FailedLogins.ContainsKey(accountId))
            FailedLogins[accountId] = new List<DateTime>();

        FailedLogins[accountId].Add(DateTime.UtcNow);
    }

    public static void SimulatePasswordReset(string accountId)
    {
        PasswordResets[accountId] = DateTime.UtcNow;
    }
}