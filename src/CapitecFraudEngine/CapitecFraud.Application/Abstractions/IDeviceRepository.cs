namespace CapitecFraud.Application.Abstractions;

public interface IDeviceRepository
{
    Task<string> GetDeviceId(string accountId);
    Task<bool> IsNewDevice(string deviceId);
    Task<int> CountAccountsUsingDevice(string deviceId);
    Task<string> GetDeviceIpAddress(string transactionAccountId);
    Task<int> CountIpAddresses(string deviceId);
}