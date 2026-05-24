using CapitecFraud.Application.Engines;
using CapitecFraud.Domain.Entities;
using CapitecFraud.Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;

namespace CapitecFraud.Tests.FraudEngineTests;

public class FraudEngineTest
{
    private readonly Mock<ILogger<FraudEngine>> _mockLogger;
    private readonly FraudEngine _fraudEngine;

    public FraudEngineTest()
    {
        _mockLogger = new Mock<ILogger<FraudEngine>>();
        _fraudEngine = new FraudEngine(_mockLogger.Object);
    }

    #region Decide Method Tests

    [Fact]
    public void Decide_WhenScoreGreaterThanOrEqualBlockThreshold_ReturnsBlock()
    {
        // Arrange
        var config = new FraudDecisionRuleConfig { BlockThreshold = 100, ReviewThreshold = 50 };
        var score = 150;

        // Act
        var decision = FraudEngine.Decide(score, config);

        // Assert
        Assert.Equal(FraudDecision.BLOCK, decision);
    }

    [Fact]
    public void Decide_WhenScoreEqualsBlockThreshold_ReturnsBlock()
    {
        // Arrange
        var config = new FraudDecisionRuleConfig { BlockThreshold = 100, ReviewThreshold = 50 };
        var score = 100;

        // Act
        var decision = FraudEngine.Decide(score, config);

        // Assert
        Assert.Equal(FraudDecision.BLOCK, decision);
    }

    [Fact]
    public void Decide_WhenScoreBetweenReviewAndBlockThreshold_ReturnsReview()
    {
        // Arrange
        var config = new FraudDecisionRuleConfig { BlockThreshold = 100, ReviewThreshold = 50 };
        var score = 75;

        // Act
        var decision = FraudEngine.Decide(score, config);

        // Assert
        Assert.Equal(FraudDecision.REVIEW, decision);
    }

    [Fact]
    public void Decide_WhenScoreEqualsReviewThreshold_ReturnsReview()
    {
        // Arrange
        var config = new FraudDecisionRuleConfig { BlockThreshold = 100, ReviewThreshold = 50 };
        var score = 50;

        // Act
        var decision = FraudEngine.Decide(score, config);

        // Assert
        Assert.Equal(FraudDecision.REVIEW, decision);
    }

    [Fact]
    public void Decide_WhenScoreLessThanReviewThreshold_ReturnsAllow()
    {
        // Arrange
        var config = new FraudDecisionRuleConfig { BlockThreshold = 100, ReviewThreshold = 50 };
        var score = 25;

        // Act
        var decision = FraudEngine.Decide(score, config);

        // Assert
        Assert.Equal(FraudDecision.ALLOW, decision);
    }

    [Fact]
    public void Decide_WhenScoreIsZero_ReturnsAllow()
    {
        // Arrange
        var config = new FraudDecisionRuleConfig { BlockThreshold = 100, ReviewThreshold = 50 };
        var score = 0;

        // Act
        var decision = FraudEngine.Decide(score, config);

        // Assert
        Assert.Equal(FraudDecision.ALLOW, decision);
    }

    [Fact]
    public void Decide_WithHighThresholds_WhenScoreExceedsBlock_ReturnsBlock()
    {
        // Arrange
        var config = new FraudDecisionRuleConfig { BlockThreshold = 1000, ReviewThreshold = 500 };
        var score = 1500;

        // Act
        var decision = FraudEngine.Decide(score, config);

        // Assert
        Assert.Equal(FraudDecision.BLOCK, decision);
    }

    [Fact]
    public void Decide_WithLowThresholds_WhenScoreInReviewRange_ReturnsReview()
    {
        // Arrange
        var config = new FraudDecisionRuleConfig { BlockThreshold = 10, ReviewThreshold = 5 };
        var score = 7;

        // Act
        var decision = FraudEngine.Decide(score, config);

        // Assert
        Assert.Equal(FraudDecision.REVIEW, decision);
    }

    [Fact]
    public void Decide_WithIdenticalThresholds_WhenScoreEqualsThreshold_ReturnsBlock()
    {
        // Arrange
        var config = new FraudDecisionRuleConfig { BlockThreshold = 50, ReviewThreshold = 50 };
        var score = 50;

        // Act
        var decision = FraudEngine.Decide(score, config);

        // Assert
        Assert.Equal(FraudDecision.BLOCK, decision);
    }

    #endregion
}