using CapitecFraud.Application.Abstractions;
using CapitecFraud.Domain.Models;

namespace CapitecFraud.Application.Features.Providers;

public class DeviceFeatureProvider(IDeviceRepository deviceRepository) : IFeatureProvider
{
    public async Task EnrichAsync(FraudEvaluationContext context)
    {
        var deviceId = await deviceRepository.GetDeviceId(context.Transaction.AccountId);
        var ipAddress = await deviceRepository.GetDeviceIpAddress(context.Transaction.AccountId);

        context.DeviceId = deviceId;
        context.IpAddress = ipAddress;
        context.IsNewDevice = await deviceRepository.IsNewDevice(deviceId);
        context.DeviceAccountCount = await deviceRepository.CountAccountsUsingDevice(deviceId);
        context.IpAddressCount = await deviceRepository.CountIpAddresses(deviceId);
    }
}