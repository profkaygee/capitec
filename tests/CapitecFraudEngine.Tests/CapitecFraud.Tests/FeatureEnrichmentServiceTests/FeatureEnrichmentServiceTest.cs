using System.Collections.Generic;
using System.Threading.Tasks;
using CapitecFraud.Application.Abstractions;
using CapitecFraud.Application.Models;
using CapitecFraud.Application.Services;
using CapitecFraud.Domain.Models;
using FluentAssertions;
using Moq;
using Xunit;

public class FeatureEnrichmentServiceTests
{
    [Fact]
    public async Task BuildAsync_Should_Return_Context_With_Transaction()
    {
        // Arrange
        var providers = new List<Mock<IFeatureProvider>>();

        var service = new FeatureEnrichmentService(new List<IFeatureProvider>());

        var txn = new TransactionMessage
        {
            TransactionId = Guid.NewGuid(),
            AccountId = "ACC123"
        };

        // Act
        var result = await service.BuildAsync(txn);

        // Assert
        result.Transaction.Should().Be(txn);
    }

    [Fact]
    public async Task BuildAsync_Should_Call_All_Providers()
    {
        // Arrange
        var provider1 = new Mock<IFeatureProvider>();
        var provider2 = new Mock<IFeatureProvider>();

        provider1
            .Setup(p => p.EnrichAsync(It.IsAny<FraudEvaluationContext>()))
            .Returns(Task.CompletedTask);

        provider2
            .Setup(p => p.EnrichAsync(It.IsAny<FraudEvaluationContext>()))
            .Returns(Task.CompletedTask);

        var service = new FeatureEnrichmentService(new[]
        {
            provider1.Object,
            provider2.Object
        });

        var txn = new TransactionMessage
        {
            TransactionId = Guid.NewGuid(),
            AccountId = "ACC999"
        };

        // Act
        await service.BuildAsync(txn);

        // Assert
        provider1.Verify(p => p.EnrichAsync(It.IsAny<FraudEvaluationContext>()), Times.Once);
        provider2.Verify(p => p.EnrichAsync(It.IsAny<FraudEvaluationContext>()), Times.Once);
    }

    [Fact]
    public async Task BuildAsync_Should_Pass_Same_Context_To_All_Providers()
    {
        // Arrange
        FraudEvaluationContext firstContext = null;
        FraudEvaluationContext secondContext = null;

        var provider1 = new Mock<IFeatureProvider>();
        var provider2 = new Mock<IFeatureProvider>();

        provider1
            .Setup(p => p.EnrichAsync(It.IsAny<FraudEvaluationContext>()))
            .Callback<FraudEvaluationContext>(ctx => firstContext = ctx)
            .Returns(Task.CompletedTask);

        provider2
            .Setup(p => p.EnrichAsync(It.IsAny<FraudEvaluationContext>()))
            .Callback<FraudEvaluationContext>(ctx => secondContext = ctx)
            .Returns(Task.CompletedTask);

        var service = new FeatureEnrichmentService(new[]
        {
            provider1.Object,
            provider2.Object
        });

        var txn = new TransactionMessage
        {
            TransactionId = Guid.NewGuid(),
            AccountId = "ACC777"
        };

        // Act
        await service.BuildAsync(txn);

        // Assert
        firstContext.Should().NotBeNull();
        secondContext.Should().NotBeNull();

        firstContext.Should().BeSameAs(secondContext, "all providers must enrich the same context instance");
    }

    [Fact]
    public async Task BuildAsync_Should_Run_Providers_In_Order()
    {
        // Arrange
        var executionOrder = new List<int>();

        var provider1 = new Mock<IFeatureProvider>();
        var provider2 = new Mock<IFeatureProvider>();

        provider1
            .Setup(p => p.EnrichAsync(It.IsAny<FraudEvaluationContext>()))
            .Returns<FraudEvaluationContext>(ctx =>
            {
                executionOrder.Add(1);
                return Task.CompletedTask;
            });

        provider2
            .Setup(p => p.EnrichAsync(It.IsAny<FraudEvaluationContext>()))
            .Returns<FraudEvaluationContext>(ctx =>
            {
                executionOrder.Add(2);
                return Task.CompletedTask;
            });

        var service = new FeatureEnrichmentService(new[]
        {
            provider1.Object,
            provider2.Object
        });

        var txn = new TransactionMessage
        {
            TransactionId = Guid.NewGuid(),
            AccountId = "ACCORDER"
        };

        // Act
        await service.BuildAsync(txn);

        // Assert
        executionOrder.Should().Equal(1, 2);
    }
}