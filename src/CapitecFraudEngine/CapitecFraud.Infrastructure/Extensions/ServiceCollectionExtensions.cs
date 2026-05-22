using CapitecFraud.Application.Abstractions;
using CapitecFraud.Application.Engines;
using CapitecFraud.Application.Features.Providers;
using CapitecFraud.Application.Services;
using CapitecFraud.Domain.Entities;
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

        // Services
        services.AddScoped<IGeoService, GeoService>();

        // Feature providers
        services.AddScoped<IFeatureProvider, VelocityFeatureProvider>();
        services.AddScoped<IFeatureProvider, DeviceFeatureProvider>();
        services.AddScoped<IFeatureProvider, GeoFeatureProvider>();
        services.AddScoped<IFeatureProvider, AccountBehaviorFeatureProvider>();
        services.AddScoped<IFeatureProvider, AuthenticationFeatureProvider>();

        // Core engines
        services.AddScoped<FeatureEnrichmentService>();
        services.AddScoped<FraudEngine>();

        return services;
    }
}