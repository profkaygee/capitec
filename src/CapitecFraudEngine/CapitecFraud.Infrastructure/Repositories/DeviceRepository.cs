using CapitecFraud.Application.Abstractions;
using CapitecFraud.Domain.Models;
using CapitecFraud.Infrastructure.Persistence;
using CapitecFraud.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace CapitecFraud.Infrastructure.Repositories;

public class DeviceRepository : IDeviceRepository
{
    private readonly CapitecFraudDbContext _db;
    private readonly DeviceFingerprintService _fingerprint;

    public DeviceRepository(CapitecFraudDbContext db, DeviceFingerprintService fingerprint)
    {
        _db = db;
        _fingerprint = fingerprint;
    }

    public async Task<string> GetDeviceId(string accountId)
    {
        // In real life: pass IP + UserAgent from request
        var ip = "127.0.0.1";
        var userAgent = "demo-agent";

        var fingerprint = _fingerprint.Generate(accountId, ip, userAgent);

        var device = await _db.Devices
            .Include(d => d.AccountDevices)
            .FirstOrDefaultAsync(d => d.Fingerprint == fingerprint);

        if (device == null)
        {
            device = new Device
            {
                Fingerprint = fingerprint,
                FirstSeen = DateTime.UtcNow,
                LastSeen = DateTime.UtcNow
            };

            _db.Devices.Add(device);
            await _db.SaveChangesAsync();
        }

        // Link account to device if not already linked
        var exists = await _db.AccountDevices
            .AnyAsync(ad => ad.AccountId == accountId && ad.DeviceId == device.Id);

        if (exists) 
            return device.Id;
        
        _db.AccountDevices.Add(new AccountDevice
        {
            AccountId = accountId,
            DeviceId = device.Id
        });

        await _db.SaveChangesAsync();
        return device.Id;
    }

    public async Task<bool> IsNewDevice(string deviceId)
    {
        var device = await _db.Devices
            .Include(d => d.AccountDevices)
            .FirstOrDefaultAsync(d => d.Id == deviceId);

        if (device == null) 
            return true;

        // If only 1 account has ever used it → likely new
        return device.AccountDevices.Count <= 1;
    }

    public async Task<int> CountAccountsUsingDevice(string deviceId)
    {
        return await _db.AccountDevices
            .Where(ad => ad.DeviceId == deviceId)
            .Select(ad => ad.AccountId)
            .Distinct()
            .CountAsync();
    }
}