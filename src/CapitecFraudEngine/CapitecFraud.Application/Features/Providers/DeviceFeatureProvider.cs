using CapitecFraud.Application.Abstractions;
using CapitecFraud.Application.Models;

namespace CapitecFraud.Application.Features.Providers;

public class DeviceFeatureProvider : IFeatureProvider
{
    private readonly IDeviceRepository _repo;

    public DeviceFeatureProvider(IDeviceRepository repo)
    {
        _repo = repo;
    }

    public async Task EnrichAsync(FraudEvaluationContext context)
    {
        var deviceId = await _repo.GetDeviceId(context.Transaction.AccountId);

        context.DeviceId = deviceId;
        context.IsNewDevice = await _repo.IsNewDevice(deviceId);
        context.DeviceAccountCount = await _repo.CountAccountsUsingDevice(deviceId);
    }
}