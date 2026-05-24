using CapitecFraud.Domain.Entities;
using CapitecFraud.Domain.Enums;
using CapitecFraud.Infrastructure.Persistence;
using CapitecFraud.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

public class FraudResultRepositoryTests
{
    private CapitecFraudDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<CapitecFraudDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new CapitecFraudDbContext(options);
    }

    private FraudResultRepository CreateRepository(CapitecFraudDbContext context)
        => new FraudResultRepository(context);

    [Fact]
    public async Task AddAsync_Should_Save_FraudResult_To_Database()
    {
        // Arrange
        var context = CreateContext();
        var repo = CreateRepository(context);

        var result = new FraudResult
        {
            Id = 1,
            TransactionId = Guid.NewGuid(),
            Decision = FraudDecision.ALLOW,
            RiskScore = 45
        };

        // Act
        await repo.AddAsync(result);

        // Assert
        var saved = await context.FraudResults.FirstOrDefaultAsync();

        saved.Should().NotBeNull();
        saved!.TransactionId.Should().Be(result.TransactionId);
        saved.Decision.Should().Be(FraudDecision.ALLOW);
        saved.RiskScore.Should().Be(45);
    }

    [Fact]
    public async Task AddAsync_Should_Persist_Entity_Count()
    {
        // Arrange
        var context = CreateContext();
        var repo = CreateRepository(context);

        var result1 = new FraudResult
        {
            Id = 1,
            TransactionId = Guid.NewGuid(),
            Decision = FraudDecision.BLOCK,
            RiskScore = 95
        };

        var result2 = new FraudResult
        {
            Id = 2,
            TransactionId = Guid.NewGuid(),
            //AccountId = "ACC2",
            Decision = FraudDecision.REVIEW,
            RiskScore = 50,
            ResultDate = DateTime.UtcNow
        };

        // Act
        await repo.AddAsync(result1);
        await repo.AddAsync(result2);

        // Assert
        var count = await context.FraudResults.CountAsync();
        count.Should().Be(2);
    }

    [Fact]
    public async Task AddAsync_Should_Save_All_Fields_Correctly()
    {
        // Arrange
        var context = CreateContext();
        var repo = CreateRepository(context);

        var transactionId = Guid.NewGuid();
        //var accountId = "ACC999";

        var result = new FraudResult
        {
            Id = 1,
            TransactionId = transactionId,
            //AccountId = accountId,
            Decision = FraudDecision.REVIEW,
            RiskScore = 65,
            ResultDate = DateTime.UtcNow
        };

        // Act
        await repo.AddAsync(result);

        // Assert
        var saved = await context.FraudResults.FirstAsync();

        saved.Should().NotBeNull();
        saved.TransactionId.Should().Be(transactionId);
        //saved.AccountId.Should().Be(accountId);
        saved.Decision.Should().Be(FraudDecision.REVIEW);
        saved.RiskScore.Should().Be(65);
        saved.ResultDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}