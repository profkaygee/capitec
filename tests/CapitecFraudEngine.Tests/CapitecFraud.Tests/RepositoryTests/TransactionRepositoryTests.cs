using CapitecFraud.Domain.Models;
using CapitecFraud.Infrastructure.Persistence;
using CapitecFraud.Infrastructure.Repositories;
using CapitecFraud.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace CapitecFraud.Tests.RepositoryTests;

public class TransactionRepositoryTests
{
    [Fact]
    public async Task CountTransactions_WithNonExistentAccount_ReturnsZero()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CapitecFraudDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new CapitecFraudDbContext(options);

        var clock = new ClockService()
        {
            UtcNow = DateTime.Now
        };
        var repository = new TransactionRepository(context, clock);

        var timeWindow = TimeSpan.FromHours(1);

        // Act
        var result = await repository.CountTransactions("non-existent-account", timeWindow);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task CountTransactions_WithNoTransactions_ReturnsZero()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CapitecFraudDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new CapitecFraudDbContext(options);
        var clock = new ClockService()
        {
            UtcNow = DateTime.Now
        };
        
        var repository = new TransactionRepository(context,clock);
        var timeWindow = TimeSpan.FromHours(1);

        // Act
        var result = await repository.CountTransactions("account-123", timeWindow);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task CountTransactions_WithTransactionsWithinWindow_ReturnsCount()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CapitecFraudDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new CapitecFraudDbContext(options);
        var clock = new ClockService()
        {
            UtcNow = DateTime.Now
        };

        var now = clock.UtcNow;

        context.Transactions.AddRange(
            new TransactionMessage { AccountId = "account-123", Timestamp = now.AddMinutes(-30) },
            new TransactionMessage { AccountId = "account-123", Timestamp = now.AddMinutes(-15) }
        );

        await context.SaveChangesAsync();

        var repository = new TransactionRepository(context, clock);

        // Act
        var result = await repository.CountTransactions("account-123", TimeSpan.FromHours(1));

        // Assert
        Assert.Equal(2, result);
    }

    [Fact]
    public async Task CountTransactions_WithTransactionsOutsideWindow_ReturnsZero()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CapitecFraudDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new CapitecFraudDbContext(options);
        var clock = new ClockService()
        {
            UtcNow = DateTime.Now
        };

        var now = DateTime.UtcNow;

        context.Transactions.AddRange(
            new TransactionMessage { AccountId = "account-123", Timestamp = now.AddHours(-2) },
            new TransactionMessage { AccountId = "account-123", Timestamp = now.AddHours(-3) }
        );

        await context.SaveChangesAsync();

        var repository = new TransactionRepository(context,clock);

        var timeWindow = TimeSpan.FromHours(1);

        // Act
        var result = await repository.CountTransactions("account-123", timeWindow);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task CountTransactions_WithMixedTransactions_CountsOnlyWithinWindow()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CapitecFraudDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new CapitecFraudDbContext(options);
        var clock = new ClockService
        {
            UtcNow = DateTime.Now
        };

        var now = clock.UtcNow;

        context.Transactions.AddRange(
            new TransactionMessage { AccountId = "account-123", Timestamp = now.AddMinutes(-30) },
            new TransactionMessage { AccountId = "account-123", Timestamp = now.AddMinutes(-20) },
            new TransactionMessage { AccountId = "account-123", Timestamp = now.AddHours(-2) }
        );

        await context.SaveChangesAsync();
        
        var repository = new TransactionRepository(context, clock);

        var timeWindow = TimeSpan.FromHours(1);

        // Act
        var result = await repository.CountTransactions("account-123", timeWindow);

        // Assert
        Assert.Equal(2, result);
    }

    [Fact]
    public async Task CountTransactions_WithTransactionAtWindowBoundary_IncludesTransaction()
    {
        // Arrange
        var fixedNow = new DateTime(2026, 01, 01, 12, 00, 00);

        var clock = new ClockService()
        {
            UtcNow = fixedNow
        };

        var options = new DbContextOptionsBuilder<CapitecFraudDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new CapitecFraudDbContext(options);

        var repository = new TransactionRepository(context, clock);

        var timeWindow = TimeSpan.FromHours(2);

        var boundary = fixedNow.Subtract(timeWindow);

        context.Transactions.AddRange(
            new TransactionMessage { AccountId = "account-123", Timestamp = boundary },
            new TransactionMessage { AccountId = "account-123", Timestamp = fixedNow.AddMinutes(-30) }
        );

        await context.SaveChangesAsync();

        // Act
        var result = await repository.CountTransactions("account-123", timeWindow);

        // Assert
        Assert.Equal(2, result);
    }

    [Fact]
    public async Task CountTransactions_FiltersOnlyByAccountId()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CapitecFraudDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new CapitecFraudDbContext(options);
        var clock = new ClockService()
        {
            UtcNow = DateTime.Now
        };

        var now = clock.UtcNow;

        context.Transactions.AddRange(
            new TransactionMessage
            {
                AccountId = "account-123",
                Timestamp = now.AddMinutes(-30)
            },
            new TransactionMessage
            {
                AccountId = "account-123",
                Timestamp = now.AddHours(-3)
            }
        );

        await context.SaveChangesAsync();
        
        var repository = new TransactionRepository(context,clock);

        var timeWindow = TimeSpan.FromHours(1);

        // Act
        var result = await repository.CountTransactions("account-123", timeWindow);

        // Assert
        Assert.Equal(1, result);
    }
}