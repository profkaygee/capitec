namespace CapitecFraud.Application.Abstractions;

public interface IAuthRepository
{
    Task<int> GetFailedLogins(string accountId, TimeSpan window);
    Task<int> MinutesSincePasswordReset(string accountId);
}