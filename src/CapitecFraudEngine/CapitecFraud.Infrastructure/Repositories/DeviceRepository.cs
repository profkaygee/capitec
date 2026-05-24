using Bogus;
using Bogus.DataSets;
using CapitecFraud.Application.Abstractions;
using CapitecFraud.Domain.Abstractions.Services;
using CapitecFraud.Domain.Models;
using CapitecFraud.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CapitecFraud.Infrastructure.Repositories;

public class DeviceRepository(CapitecFraudDbContext database, 
    IDeviceFingerprintService fingerprintService) : IDeviceRepository
{
    public async Task<string> GetDeviceId(string accountId)
    {
        Randomizer.Seed = new Random(5);
        var faker = new Faker();
        
        // In real life: pass IP + UserAgent from request
        var ip = faker.Internet.Ip();
        var userAgent = faker.Internet.UserAgent();

        var fingerprint = fingerprintService.Generate(accountId, ip, userAgent);

        var device = await database.Devices
            .Include(d => d.AccountDevices)
            .FirstOrDefaultAsync(d => d.Fingerprint == fingerprint);

        if (device == null)
        {
            device = new Device
            {
                Fingerprint = fingerprint,
                IpAddress = ip,
                FirstSeen = DateTime.UtcNow,
                LastSeen = DateTime.UtcNow
            };

            database.Devices.Add(device);
            await database.SaveChangesAsync();
        }

        // Link account to device if not already linked
        var exists = await database.AccountDevices
            .AnyAsync(ad => ad.AccountId == accountId && ad.DeviceId == device.Id);

        if (exists)
            return device.Id;

        database.AccountDevices.Add(new AccountDevice
        {
            AccountId = accountId,
            DeviceId = device.Id
        });

        await database.SaveChangesAsync();
        return device.Id;
    }

    public async Task<bool> IsNewDevice(string deviceId)
    {
        var device = await database.Devices
            .Include(d => d.AccountDevices)
            .FirstOrDefaultAsync(d => d.Id == deviceId);

        if (device == null)
            return true;

        return device.AccountDevices.Count <= 1;
    }

    public async Task<int> CountAccountsUsingDevice(string deviceId)
    {
        return await database.AccountDevices
            .Where(ad => ad.DeviceId == deviceId)
            .Select(ad => ad.AccountId)
            .Distinct()
            .CountAsync();
    }

    public async Task<string> GetDeviceIpAddress(string transactionAccountId)
    {
        return await database.AccountDevices
            .Where(ad => ad.AccountId == transactionAccountId)
            .Select(ad => ad.Device.IpAddress)
            .FirstOrDefaultAsync();
    }

    public async Task<int> CountIpAddresses(string deviceId)
    {
        return await database.Devices
            .Where(d => d.Id == deviceId)
            .Select(d => d.IpAddress)
            .Distinct()
            .CountAsync();
    }
}