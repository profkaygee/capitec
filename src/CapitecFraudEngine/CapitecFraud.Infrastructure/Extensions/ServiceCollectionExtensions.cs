using CapitecFraud.Application.Abstractions;
using CapitecFraud.Application.Engines;
using CapitecFraud.Application.Features.Providers;
using CapitecFraud.Application.Services;
using CapitecFraud.Application.Validation;
using CapitecFraud.Domain.Abstractions.Repositories;
using CapitecFraud.Domain.Abstractions.Services;
using CapitecFraud.Infrastructure.Messaging;
using CapitecFraud.Infrastructure.Repositories;
using CapitecFraud.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CapitecFraud.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFraudServices(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<DeviceFingerprintService>();
        services.AddScoped<IDeviceRepository, DeviceRepository>();
        services.AddScoped<IAuthRepository, FakeAuthRepository>();
        services.AddScoped<IRuleRepository, RuleRepository>();
        services.AddScoped<IFraudResultRepository, FraudResultRepository>();

        // Services
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IGeoService, GeoService>();
        services.AddScoped<IClockService, ClockService>();
        services.AddScoped<IDeviceFingerprintService, DeviceFingerprintService>();

        // Feature providers
        services.AddScoped<IFeatureProvider, VelocityFeatureProvider>();
        services.AddScoped<IFeatureProvider, DeviceFeatureProvider>();
        services.AddScoped<IFeatureProvider, GeoFeatureProvider>();
        services.AddScoped<IFeatureProvider, AccountBehaviorFeatureProvider>();
        services.AddScoped<IFeatureProvider, AuthenticationFeatureProvider>();

        // Core engines
        services.AddScoped<FeatureEnrichmentService>();
        services.AddScoped<FraudEngine>();
        services.AddScoped<TransactionGuard>();
        
        // Queue consumer
        services.AddScoped<ITransactionQueue, RabbitMqTransactionQueue>();

        return services;
    }
}