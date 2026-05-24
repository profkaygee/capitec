using CapitecFraud.Application.Models;
using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using CapitecFraud.Infrastructure.Repositories;
using CapitecFraud.Infrastructure.Persistence;
using CapitecFraud.Domain.Entities;

namespace CapitecFraud.Infrastructure.Tests.Repositories;

public class TransactionRepositoryTests
{
    [Fact]
    public async Task CountTransactions_WithNonExistentAccount_ReturnsZero()
    {
        // Arrange
        var mockContext = new Mock<CapitecFraudDbContext>();
        var mockTransactionSet = new Mock<DbSet<TransactionMessage>>();
        var queryable = new List<TransactionMessage>().AsQueryable().BuildMockDbSet();
        
        mockContext.Setup(c => c.Transactions).Returns(queryable.Object);
        var repository = new TransactionRepository(mockContext.Object);
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
        var mockContext = new Mock<CapitecFraudDbContext>();
        var transactions = new List<TransactionMessage>().AsQueryable().BuildMockDbSet();
        
        mockContext.Setup(c => c.Transactions).Returns(transactions.Object);
        var repository = new TransactionRepository(mockContext.Object);
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
        var now = DateTime.UtcNow;
        var transactions = new List<TransactionMessage>
        {
            new TransactionMessage { AccountId = "account-123", Timestamp = now.AddMinutes(-30) },
            new TransactionMessage { AccountId = "account-123", Timestamp = now.AddMinutes(-15) }
        }.AsQueryable().BuildMockDbSet();
        
        var mockContext = new Mock<CapitecFraudDbContext>();
        mockContext.Setup(c => c.Transactions).Returns(transactions.Object);
        var repository = new TransactionRepository(mockContext.Object);
        var timeWindow = TimeSpan.FromHours(1);

        // Act
        var result = await repository.CountTransactions("account-123", timeWindow);

        // Assert
        Assert.Equal(2, result);
    }

    [Fact]
    public async Task CountTransactions_WithTransactionsOutsideWindow_ReturnsZero()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var transactions = new List<TransactionMessage>
        {
            new TransactionMessage { AccountId = "account-123", Timestamp = now.AddHours(-2) },
            new TransactionMessage { AccountId = "account-123", Timestamp = now.AddHours(-3) }
        }.AsQueryable().BuildMockDbSet();
        
        var mockContext = new Mock<CapitecFraudDbContext>();
        mockContext.Setup(c => c.Transactions).Returns(transactions.Object);
        var repository = new TransactionRepository(mockContext.Object);
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
        var now = DateTime.UtcNow;
        var transactions = new List<TransactionMessage>
        {
            new TransactionMessage { AccountId = "account-123", Timestamp = now.AddMinutes(-30) },  // within
            new TransactionMessage { AccountId = "account-123", Timestamp = now.AddMinutes(-20) },  // within
            new TransactionMessage { AccountId = "account-123", Timestamp = now.AddHours(-2) }      // outside
        }.AsQueryable().BuildMockDbSet();
        
        var mockContext = new Mock<CapitecFraudDbContext>();
        mockContext.Setup(c => c.Transactions).Returns(transactions.Object);
        var repository = new TransactionRepository(mockContext.Object);
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
        var now = DateTime.UtcNow;
        var timeWindow = TimeSpan.FromHours(1);
        var boundaryTime = now.Subtract(timeWindow);
        
        var transactions = new List<TransactionMessage>
        {
            new TransactionMessage { AccountId = "account-123", Timestamp = boundaryTime },
            new TransactionMessage { AccountId = "account-123", Timestamp = now.AddMinutes(-30) }
        }.AsQueryable().BuildMockDbSet();
        
        var mockContext = new Mock<CapitecFraudDbContext>();
        mockContext.Setup(c => c.Transactions).Returns(transactions.Object);
        var repository = new TransactionRepository(mockContext.Object);

        // Act
        var result = await repository.CountTransactions("account-123", timeWindow);

        // Assert
        Assert.Equal(2, result);
    }

    [Fact]
    public async Task CountTransactions_FiltersOnlyByAccountId()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var transactions = new List<TransactionMessage>
        {
            new TransactionMessage() { AccountId = "account-123", Timestamp = now.AddMinutes(-30) },
            new TransactionMessage() { AccountId = "account-456", Timestamp = now.AddMinutes(-20) }
        }.AsQueryable().BuildMockDbSet();
        
        var mockContext = new Mock<CapitecFraudDbContext>();
        mockContext.Setup(c => c.Transactions).Returns(transactions.Object);
        var repository = new TransactionRepository(mockContext.Object);
        var timeWindow = TimeSpan.FromHours(1);

        // Act
        var result = await repository.CountTransactions("account-123", timeWindow);

        // Assert
        Assert.Equal(1, result);
    }
}