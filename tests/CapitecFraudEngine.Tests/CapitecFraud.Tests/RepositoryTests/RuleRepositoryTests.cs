using CapitecFraud.Domain.Entities;
using CapitecFraud.Infrastructure.Persistence;
using CapitecFraud.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace CapitecFraud.Tests.RepositoryTests;

public class RuleRepositoryTests
{
    [Fact]
    public async Task GetDecisionConfigAsync_WithExistingConfig_ReturnsConfig()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CapitecFraudDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new CapitecFraudDbContext(options);

        context.FraudDecisionRuleConfigs.Add(new FraudDecisionRuleConfig
        {
            Id = 1,
            BlockThreshold = 80,
            ReviewThreshold = 50,
            Name = "Default"
        });

        await context.SaveChangesAsync();

        var repository = new RuleRepository(context);

        // Act
        var result = await repository.GetDecisionConfigAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(80, result.BlockThreshold);
        Assert.Equal(50, result.ReviewThreshold);
    }

    [Fact]
    public async Task GetDecisionConfigAsync_WithNoConfig_ReturnsNull()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CapitecFraudDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new CapitecFraudDbContext(options);

        var repository = new RuleRepository(context);

        // Act
        var result = await repository.GetDecisionConfigAsync();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetDecisionConfigAsync_WithMultipleConfigs_ReturnsFirst()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CapitecFraudDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new CapitecFraudDbContext(options);

        context.FraudDecisionRuleConfigs.AddRange(
            new FraudDecisionRuleConfig
            {
                Id = 1,
                BlockThreshold = 90,
                ReviewThreshold = 60,
                Name = "Config-1"
            },
            new FraudDecisionRuleConfig
            {
                Id = 2,
                BlockThreshold = 70,
                ReviewThreshold = 40,
                Name = "Config-2"
            }
        );

        await context.SaveChangesAsync();

        var repository = new RuleRepository(context);

        // Act
        var result = await repository.GetDecisionConfigAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result!.Id);              // first inserted
        Assert.Equal(90, result.BlockThreshold);
        Assert.Equal(60, result.ReviewThreshold);
    }

    [Fact]
    public async Task GetDecisionConfigAsync_Returns_Config()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CapitecFraudDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new CapitecFraudDbContext(options);

        context.FraudDecisionRuleConfigs.Add(new FraudDecisionRuleConfig
        {
            Id = 1,
            Name = "Default",
            ReviewThreshold = 50,
            BlockThreshold = 80
        });

        await context.SaveChangesAsync();

        var repository = new RuleRepository(context);

        // Act
        var result = await repository.GetDecisionConfigAsync();

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Default");
    }
}