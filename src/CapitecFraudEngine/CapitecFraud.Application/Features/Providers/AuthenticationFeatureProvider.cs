using CapitecFraud.Application.Abstractions;
using CapitecFraud.Application.Models;

namespace CapitecFraud.Application.Features.Providers;

public class AuthenticationFeatureProvider : IFeatureProvider
{
    private readonly IAuthRepository _auth;

    public AuthenticationFeatureProvider(IAuthRepository auth)
    {
        _auth = auth;
    }

    public async Task EnrichAsync(FraudEvaluationContext context)
    {
        var id = context.Transaction.AccountId;

        context.FailedLoginAttempts =
            await _auth.GetFailedLogins(id, TimeSpan.FromMinutes(15));

        context.PasswordResetLastMinutes =
            await _auth.MinutesSincePasswordReset(id);
    }
}