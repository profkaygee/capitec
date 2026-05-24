using CapitecFraud.Application.Abstractions;
using CapitecFraud.Domain.Models;

namespace CapitecFraud.Application.Features.Providers;

public class AuthenticationFeatureProvider(IAuthRepository authRepository) : IFeatureProvider
{
    public async Task EnrichAsync(FraudEvaluationContext context)
    {
        var id = context.Transaction.AccountId;

        context.FailedLoginAttempts =
            await authRepository.GetFailedLogins(id, TimeSpan.FromMinutes(15));

        context.PasswordResetLastMinutes =
            await authRepository.MinutesSincePasswordReset(id);
    }
}