using CapitecFraud.Application.Abstractions;
using CapitecFraud.Application.Models;

namespace CapitecFraud.Application.Features.Providers;

public class DeviceFeatureProvider(IDeviceRepository deviceRepository) : IFeatureProvider
{
    public async Task EnrichAsync(FraudEvaluationContext context)
    {
        var deviceId = await deviceRepository.GetDeviceId(context.Transaction.AccountId);

        context.DeviceId = deviceId;
        context.IsNewDevice = await deviceRepository.IsNewDevice(deviceId);
        context.DeviceAccountCount = await deviceRepository.CountAccountsUsingDevice(deviceId);
    }
}