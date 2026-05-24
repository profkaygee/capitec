using System.Text.Json;
using CapitecFraud.Domain.Entities;
using CapitecFraud.Domain.Enums;
using CapitecFraud.Domain.Models;
using CapitecFraud.Infrastructure.Persistence;
using CapitecFraud.Infrastructure.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

public class AuditServiceTests
{
    private CapitecFraudDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<CapitecFraudDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new CapitecFraudDbContext(options);
    }

    [Fact]
    public async Task LogAsync_Should_Add_AuditLog_To_Database()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new AuditService(context);

        var transaction = new TransactionMessage
        {
            TransactionId = Guid.NewGuid(),
            AccountId = "ACC123"
        };

        var result = new FraudResult
        {
            Decision = FraudDecision.ALLOW,
            RiskScore = 50
        };

        // Act
        await service.LogAsync(transaction, result);

        // Assert
        var audit = await context.AuditLogs.FirstOrDefaultAsync();

        audit.Should().NotBeNull();
        audit!.TransactionId.Should().Be(transaction.TransactionId);
        audit.AccountId.Should().Be("ACC123");
        audit.Action.Should().Be("FRAUD_EVALUATION");
        audit.Decision.Should().Be(FraudDecision.ALLOW);
        audit.RiskScore.Should().Be(50);
    }

    [Fact]
    public async Task LogAsync_Should_Store_Serialized_Result_In_Details()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new AuditService(context);

        var transaction = new TransactionMessage
        {
            TransactionId = Guid.NewGuid(),
            AccountId = "ACC999"
        };

        var result = new FraudResult
        {
            Decision = FraudDecision.BLOCK,
            RiskScore = 100
        };

        // Act
        await service.LogAsync(transaction, result);

        // Assert
        var audit = await context.AuditLogs.FirstAsync();

        var deserialized = JsonSerializer.Deserialize<FraudResult>(audit.Details);

        deserialized.Should().NotBeNull();
        deserialized!.Decision.Should().Be(result.Decision);
        deserialized.RiskScore.Should().Be(result.RiskScore);
    }

    [Fact]
    public async Task LogAsync_Should_Set_CreatedAt_To_UtcNow()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new AuditService(context);

        var before = DateTime.UtcNow;

        var transaction = new TransactionMessage
        {
            TransactionId = Guid.NewGuid(),
            AccountId = "ACC111"
        };

        var result = new FraudResult
        {
            Decision = FraudDecision.REVIEW,
            RiskScore = 50
        };

        // Act
        await service.LogAsync(transaction, result);

        var after = DateTime.UtcNow;

        // Assert
        var audit = await context.AuditLogs.FirstAsync();

        audit.CreatedAt.Should().BeOnOrAfter(before);
        audit.CreatedAt.Should().BeOnOrBefore(after);
    }
}