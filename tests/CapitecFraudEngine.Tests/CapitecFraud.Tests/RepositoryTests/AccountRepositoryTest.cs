using CapitecFraud.Infrastructure.Persistence;
using CapitecFraud.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace CapitecFraud.Tests.RepositoryTests;

public class AccountRepositoryTests
{
    private CapitecFraudDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<CapitecFraudDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new CapitecFraudDbContext(options);
    }

    private AccountRepository CreateRepository(CapitecFraudDbContext context)
        => new AccountRepository(context);

    // ----------------------------
    // DaysSinceLastTransaction
    // ----------------------------

    [Fact]
    public async Task DaysSinceLastTransaction_Should_Return_MaxInt_When_No_Transactions()
    {
        // Arrange
        var context = CreateContext();
        var repo = CreateRepository(context);

        // Act
        var result = await repo.DaysSinceLastTransaction("ACC1");

        // Assert
        result.Should().Be(int.MaxValue);
    }

    [Fact]
    public async Task DaysSinceLastTransaction_Should_Return_Days_From_Last_Transaction()
    {
        // Arrange
        var context = CreateContext();

        var accountId = "ACC2";

        context.Transactions.Add(new()
        {
            AccountId = accountId,
            Amount = 100,
            Timestamp = DateTime.UtcNow.AddDays(-10)
        });

        context.Transactions.Add(new()
        {
            AccountId = accountId,
            Amount = 200,
            Timestamp = DateTime.UtcNow.AddDays(-2) // latest
        });

        await context.SaveChangesAsync();

        var repo = CreateRepository(context);

        // Act
        var result = await repo.DaysSinceLastTransaction(accountId);

        // Assert
        result.Should().BeInRange(1, 3); // avoids flaky exact timing issues
    }

    // ----------------------------
    // SpendingDeviation
    // ----------------------------

    [Fact]
    public async Task GetSpendingDeviation_Should_Return_Zero_When_No_Transactions()
    {
        // Arrange
        var context = CreateContext();
        var repo = CreateRepository(context);

        // Act
        var result = await repo.GetSpendingDeviation("ACC3");

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public async Task GetSpendingDeviation_Should_Calculate_Correct_Deviation()
    {
        // Arrange
        var context = CreateContext();

        var accountId = "ACC4";

        var now = DateTime.UtcNow;

        context.Transactions.Add(new()
        {
            AccountId = accountId,
            Amount = 100,
            Timestamp = now.AddDays(-5)
        });

        context.Transactions.Add(new()
        {
            AccountId = accountId,
            Amount = 100,
            Timestamp = now.AddDays(-4)
        });

        context.Transactions.Add(new()
        {
            AccountId = accountId,
            Amount = 200, // latest
            Timestamp = now.AddDays(-1)
        });

        await context.SaveChangesAsync();

        var repo = CreateRepository(context);

        // Act
        var result = await repo.GetSpendingDeviation(accountId);

        // avg = 133.33, latest = 200
        // deviation ≈ 50%

        // Assert
        result.Should().BeGreaterThan(40);
        result.Should().BeLessThan(60);
    }

    [Fact]
    public async Task GetSpendingDeviation_Should_Return_Zero_When_Average_Is_Zero()
    {
        // Arrange
        var context = CreateContext();

        var accountId = "ACC5";

        context.Transactions.Add(new()
        {
            AccountId = accountId,
            Amount = 0,
            Timestamp = DateTime.UtcNow.AddDays(-1)
        });

        await context.SaveChangesAsync();

        var repo = CreateRepository(context);

        // Act
        var result = await repo.GetSpendingDeviation(accountId);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public async Task GetSpendingDeviation_Should_Only_Consider_Last_30_Days()
    {
        // Arrange
        var context = CreateContext();

        var accountId = "ACC6";
        var now = DateTime.UtcNow;

        // old transaction (ignored)
        context.Transactions.Add(new()
        {
            AccountId = accountId,
            Amount = 999,
            Timestamp = now.AddDays(-40)
        });

        // valid transactions
        context.Transactions.Add(new()
        {
            AccountId = accountId,
            Amount = 100,
            Timestamp = now.AddDays(-10)
        });

        context.Transactions.Add(new()
        {
            AccountId = accountId,
            Amount = 200,
            Timestamp = now.AddDays(-2)
        });

        await context.SaveChangesAsync();

        var repo = CreateRepository(context);

        // Act
        var result = await repo.GetSpendingDeviation(accountId);

        // Assert
        result.Should().BeGreaterThan(0);
    }
}