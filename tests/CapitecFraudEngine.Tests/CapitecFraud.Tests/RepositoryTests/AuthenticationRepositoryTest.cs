using System.Reflection;
using CapitecFraud.Infrastructure.Repositories;
using CapitecFraud.Infrastructure.Services;
using FluentAssertions;

namespace CapitecFraud.Tests.RepositoryTests;

public class FakeAuthRepositoryTests
{
    private FakeAuthRepository CreateRepo()
    {
        var clock = new ClockService();
        return new FakeAuthRepository(clock);
    }

    private void ResetStaticState()
    {
        var type = typeof(FakeAuthRepository);

        var failedLogins = type.GetField("_failedLogins",
            BindingFlags.NonPublic | BindingFlags.Static);

        var passwordResets = type.GetField("_passwordResets",
            BindingFlags.NonPublic | BindingFlags.Static);

        failedLogins?.SetValue(null, new Dictionary<string, List<DateTime>>());
        passwordResets?.SetValue(null, new Dictionary<string, DateTime>());
    }

    [Fact]
    public async Task GetFailedLogins_Should_Return_Zero_When_No_Data()
    {
        ResetStaticState();

        var repo = CreateRepo();

        var result = await repo.GetFailedLogins("ACC1", TimeSpan.FromMinutes(10));

        result.Should().Be(0);
    }

    [Fact]
    public async Task GetFailedLogins_Should_Count_Only_Within_Window()
    {
        // Arrange
        var clock = new ClockService();
        var repository = new FakeAuthRepository(clock);

        var accountId = "ACC2";

        // 1. Add a recent login
        clock.UtcNow = DateTime.Now;
        repository.SimulateFailedLogin(accountId);

        // Act
        var result = await repository.GetFailedLogins(accountId, TimeSpan.FromMinutes(10));

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public async Task MinutesSincePasswordReset_Should_Return_IntMax_When_No_Reset()
    {
        ResetStaticState();

        var repo = CreateRepo();

        var result = await repo.MinutesSincePasswordReset("ACC3");

        result.Should().Be(int.MaxValue);
    }

    [Fact]
    public async Task MinutesSincePasswordReset_Should_Calculate_Minutes_Correctly()
    {
        // Arrange
        var clock = new ClockService();
        var repository = new FakeAuthRepository(clock);

        var accountId = "ACC4";

        clock.UtcNow = new DateTime(2026, 01, 01, 12, 00, 00);
        repository.SimulatePasswordReset(accountId);

        // move time forward 5 minutes
        clock.UtcNow = new DateTime(2026, 01, 01, 12, 05, 00);

        // Act
        var result = await repository.MinutesSincePasswordReset(accountId);

        // Assert
        result.Should().Be(5);
    }

    [Fact]
    public async Task SimulatePasswordReset_Should_Record_Reset()
    {
        // Arrange
        var clock = new ClockService();
        var repository = new FakeAuthRepository(clock);
        var accountId = "ACC5";

        // Act
        repository.SimulatePasswordReset(accountId);

        // Assert
        var result = await repository.MinutesSincePasswordReset(accountId);

        result.Should().BeLessThan(int.MaxValue);
    }

    [Fact]
    public async Task SimulateFailedLogin_Should_Record_Login()
    {
        // Arrange
        var clock = new ClockService();
        var repository = new FakeAuthRepository(clock);
        var accountId = "ACC6";

        // Act
        clock.UtcNow = DateTime.Now;
        repository.SimulateFailedLogin(accountId);

        var result = await repository.GetFailedLogins(accountId, TimeSpan.FromMinutes(10));

        // Assert
        result.Should().Be(1);
    }
}