namespace CapitecFraud.Application.Abstractions;

public interface IDeviceRepository
{
    Task<string> GetDeviceId(string accountId);
    Task<bool> IsNewDevice(string deviceId);
    Task<int> CountAccountsUsingDevice(string deviceId);
}