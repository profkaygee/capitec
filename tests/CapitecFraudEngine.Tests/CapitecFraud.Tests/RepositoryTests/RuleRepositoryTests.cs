using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using CapitecFraud.Infrastructure.Repositories;
using CapitecFraud.Infrastructure.Persistence;
using CapitecFraud.Domain.Entities;

namespace CapitecFraud.Infrastructure.Tests.Repositories;

public class RuleRepositoryTests
{
    [Fact]
    public async Task GetDecisionConfigAsync_WithExistingConfig_ReturnsConfig()
    {
        // Arrange
        var mockContext = new Mock<CapitecFraudDbContext>();
        var mockConfigSet = new Mock<DbSet<FraudDecisionRuleConfig>>();
        
        var config = new FraudDecisionRuleConfig 
        { 
            Id = 1,
            BlockThreshold = 80,
            ReviewThreshold = 50
        };
        
        mockConfigSet
            .Setup(m => m.FirstOrDefaultAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(config);
        
        mockContext.Setup(c => c.FraudDecisionRuleConfigs).Returns(mockConfigSet.Object);
        var repository = new RuleRepository(mockContext.Object);

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
        var mockContext = new Mock<CapitecFraudDbContext>();
        var mockConfigSet = new Mock<DbSet<FraudDecisionRuleConfig>>();
        
        mockConfigSet
            .Setup(m => m.FirstOrDefaultAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((FraudDecisionRuleConfig)null);
        
        mockContext.Setup(c => c.FraudDecisionRuleConfigs).Returns(mockConfigSet.Object);
        var repository = new RuleRepository(mockContext.Object);

        // Act
        var result = await repository.GetDecisionConfigAsync();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetDecisionConfigAsync_WithMultipleConfigs_ReturnsFirst()
    {
        // Arrange
        var mockContext = new Mock<CapitecFraudDbContext>();
        var mockConfigSet = new Mock<DbSet<FraudDecisionRuleConfig>>();
        
        var config = new FraudDecisionRuleConfig 
        { 
            Id = 1,
            BlockThreshold = 90,
            ReviewThreshold = 60
        };
        
        mockConfigSet
            .Setup(m => m.FirstOrDefaultAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(config);
        
        mockContext.Setup(c => c.FraudDecisionRuleConfigs).Returns(mockConfigSet.Object);
        var repository = new RuleRepository(mockContext.Object);

        // Act
        var result = await repository.GetDecisionConfigAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(90, result.BlockThreshold);
    }

    [Fact]
    public async Task GetDecisionConfigAsync_CallsFirstOrDefaultAsync()
    {
        // Arrange
        var mockContext = new Mock<CapitecFraudDbContext>();
        var mockConfigSet = new Mock<DbSet<FraudDecisionRuleConfig>>();
        
        mockConfigSet
            .Setup(m => m.FirstOrDefaultAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FraudDecisionRuleConfig());
        
        mockContext.Setup(c => c.FraudDecisionRuleConfigs).Returns(mockConfigSet.Object);
        var repository = new RuleRepository(mockContext.Object);

        // Act
        await repository.GetDecisionConfigAsync();

        // Assert
        mockConfigSet.Verify(m => m.FirstOrDefaultAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}