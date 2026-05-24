using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CapitecFraud.Infrastructure.Repositories;
using FluentAssertions;
using Xunit;

public class FakeAuthRepositoryTests
{
    private FakeAuthRepository CreateRepo() => new FakeAuthRepository();

    private void ResetStaticState()
    {
        var type = typeof(FakeAuthRepository);

        var failedLogins = type.GetField("FailedLogins",
            BindingFlags.NonPublic | BindingFlags.Static);

        var passwordResets = type.GetField("PasswordResets",
            BindingFlags.NonPublic | BindingFlags.Static);

        failedLogins!.SetValue(null, new Dictionary<string, List<DateTime>>());
        passwordResets!.SetValue(null, new Dictionary<string, DateTime>());
    }

    // ----------------------------
    // SimulateFailedLogin + GetFailedLogins
    // ----------------------------

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
        ResetStaticState();

        var accountId = "ACC2";

        // simulate old login
        var oldTime = DateTime.UtcNow.AddMinutes(-30);

        // simulate recent login using static method
        FakeAuthRepository.SimulateFailedLogin(accountId);

        // manually inject an old timestamp (to simulate history)
        var type = typeof(FakeAuthRepository);
        var field = type.GetField("FailedLogins",
            BindingFlags.NonPublic | BindingFlags.Static);

        var dict = (Dictionary<string, List<DateTime>>)field!.GetValue(null)!;
        dict[accountId].Add(oldTime);

        var repo = CreateRepo();

        // Act
        var result = await repo.GetFailedLogins(accountId, TimeSpan.FromMinutes(10));

        // Assert
        result.Should().Be(1); // only recent login counts
    }

    // ----------------------------
    // Password reset tests
    // ----------------------------

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
        ResetStaticState();

        var accountId = "ACC4";

        // simulate reset 5 minutes ago
        var type = typeof(FakeAuthRepository);
        var field = type.GetField("PasswordResets",
            BindingFlags.NonPublic | BindingFlags.Static);

        var dict = (Dictionary<string, DateTime>)field!.GetValue(null)!;

        dict[accountId] = DateTime.UtcNow.AddMinutes(-5);

        var repo = CreateRepo();

        var result = await repo.MinutesSincePasswordReset(accountId);

        result.Should().BeInRange(4, 6); // allow small timing drift
    }

    // ----------------------------
    // SimulatePasswordReset
    // ----------------------------

    [Fact]
    public void SimulatePasswordReset_Should_Store_Entry()
    {
        ResetStaticState();

        var accountId = "ACC5";

        FakeAuthRepository.SimulatePasswordReset(accountId);

        var type = typeof(FakeAuthRepository);
        var field = type.GetField("PasswordResets",
            BindingFlags.NonPublic | BindingFlags.Static);

        var dict = (Dictionary<string, DateTime>)field!.GetValue(null)!;

        dict.ContainsKey(accountId).Should().BeTrue();
    }

    [Fact]
    public void SimulateFailedLogin_Should_Create_Entry_If_Not_Exists()
    {
        ResetStaticState();

        var accountId = "ACC6";

        FakeAuthRepository.SimulateFailedLogin(accountId);

        var type = typeof(FakeAuthRepository);
        var field = type.GetField("FailedLogins",
            BindingFlags.NonPublic | BindingFlags.Static);

        var dict = (Dictionary<string, List<DateTime>>)field!.GetValue(null)!;

        dict.ContainsKey(accountId).Should().BeTrue();
        dict[accountId].Should().HaveCount(1);
    }
}