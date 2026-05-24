using CapitecFraud.Domain.Abstractions.Services;
using CapitecFraud.Domain.Models;
using CapitecFraud.Infrastructure.Persistence;
using CapitecFraud.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace CapitecFraud.Tests.RepositoryTests;

public class DeviceRepositoryTests
{
    private CapitecFraudDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<CapitecFraudDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new CapitecFraudDbContext(options);
    }

    private DeviceRepository CreateRepo(
        CapitecFraudDbContext context,
        Mock<IDeviceFingerprintService> fingerprintMock)
        => new DeviceRepository(context, fingerprintMock.Object);
    
    [Fact]
    public async Task GetDeviceId_Should_Create_Device_And_Link_Account_When_New_Device()
    {
        // Arrange
        var context = CreateContext();

        var fingerprintMock = new Mock<IDeviceFingerprintService>();
        fingerprintMock
            .Setup(f => f.Generate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns("FP-123");

        var repo = CreateRepo(context, fingerprintMock);

        var accountId = "10013115168";

        // Act
        var deviceId = await repo.GetDeviceId(accountId);

        // Assert
        var device = await context.Devices.FirstOrDefaultAsync();

        device.Should().NotBeNull();
        device!.Fingerprint.Should().Be("FP-123");

        var link = await context.AccountDevices.FirstOrDefaultAsync();

        link.Should().NotBeNull();
        link!.AccountId.Should().Be(accountId);
        link.DeviceId.Should().Be(deviceId);
    }
    
    [Fact]
    public async Task GetDeviceId_Should_Reuse_Existing_Device()
    {
        // Arrange
        var context = CreateContext();

        var fingerprintMock = new Mock<IDeviceFingerprintService>();
        fingerprintMock
            .Setup(f => f.Generate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns("FP-EXISTING");

        var existingDevice = new Device
        {
            Id = Guid.NewGuid().ToString(),
            Fingerprint = "FP-EXISTING",
            FirstSeen = DateTime.UtcNow,
            LastSeen = DateTime.UtcNow
        };

        context.Devices.Add(existingDevice);
        await context.SaveChangesAsync();

        var repo = CreateRepo(context, fingerprintMock);

        // Act
        var result = await repo.GetDeviceId("ACC1");

        // Assert
        result.Should().Be(existingDevice.Id);

        var deviceCount = await context.Devices.CountAsync();
        deviceCount.Should().Be(1); // no duplicate device created
    }
    
    [Fact]
    public async Task GetDeviceId_Should_Not_Duplicate_AccountDevice_Link()
    {
        // Arrange
        var context = CreateContext();

        var fingerprintMock = new Mock<IDeviceFingerprintService>();
        fingerprintMock
            .Setup(f => f.Generate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns("FP-999");

        var device = new Device
        {
            Id = Guid.NewGuid().ToString(),
            Fingerprint = "FP-999",
            FirstSeen = DateTime.UtcNow,
            LastSeen = DateTime.UtcNow
        };

        context.Devices.Add(device);
        await context.SaveChangesAsync();

        context.AccountDevices.Add(new AccountDevice
        {
            AccountId = "ACC1",
            DeviceId = device.Id
        });

        await context.SaveChangesAsync();

        var repo = CreateRepo(context, fingerprintMock);

        // Act
        var result = await repo.GetDeviceId("ACC1");

        // Assert
        result.Should().Be(device.Id);

        var links = await context.AccountDevices
            .Where(x => x.DeviceId == device.Id)
            .ToListAsync();

        links.Count.Should().Be(1);
    }
    
    [Fact]
    public async Task IsNewDevice_Should_Return_True_When_Device_Not_Found()
    {
        // Arrange
        var context = CreateContext();
        var repo = CreateRepo(context, new Mock<IDeviceFingerprintService>());

        // Act
        var result = await repo.IsNewDevice("NON_EXISTENT");

        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public async Task IsNewDevice_Should_Return_True_When_Only_One_Account_Uses_Device()
    {
        // Arrange
        var context = CreateContext();

        var device = new Device
        {
            Id = "DEV1",
            Fingerprint = "FP1",
            FirstSeen = DateTime.UtcNow,
            LastSeen = DateTime.UtcNow
        };

        context.Devices.Add(device);

        context.AccountDevices.Add(new AccountDevice
        {
            AccountId = "ACC1",
            DeviceId = "DEV1"
        });

        await context.SaveChangesAsync();

        var repo = CreateRepo(context, new Mock<IDeviceFingerprintService>());

        // Act
        var result = await repo.IsNewDevice("DEV1");

        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public async Task CountAccountsUsingDevice_Should_Return_Distinct_Accounts()
    {
        // Arrange
        var context = CreateContext();

        context.AccountDevices.Add(new AccountDevice
        {
            AccountId = "ACC1",
            DeviceId = "DEV1"
        });

        context.AccountDevices.Add(new AccountDevice
        {
            AccountId = "ACC2",
            DeviceId = "DEV1"
        });

        context.AccountDevices.Add(new AccountDevice
        {
            AccountId = "ACC2",
            DeviceId = "DEV1" // duplicate account
        });

        await context.SaveChangesAsync();

        var repo = CreateRepo(context, new Mock<IDeviceFingerprintService>());

        // Act
        var result = await repo.CountAccountsUsingDevice("DEV1");

        // Assert
        result.Should().Be(2);
    }
}