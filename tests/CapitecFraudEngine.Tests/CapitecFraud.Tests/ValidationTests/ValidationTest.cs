using CapitecFraud.Application.Models;
using CapitecFraud.Application.Validation;

namespace CapitecFraud.Tests.ValidationTests;

public class TransactionGuardTest
{
    private readonly TransactionGuard _guard;

    public TransactionGuardTest()
    {
        _guard = new TransactionGuard();
    }

    #region Null and Missing Field Tests

    [Fact]
    public void Validate_WithNullTransaction_ReturnsReject()
    {
        // Arrange
        TransactionMessage txn = null;

        // Act
        var result = _guard.Validate(txn);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Transaction is null", result.Reason);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WithMissingAccountId_ReturnsReject()
    {
        // Arrange
        var txn = new TransactionMessage
        {
            Id = 1,
            AccountId = null,
            Amount = 1000,
            Currency = "ZAR",
            Timestamp = DateTime.UtcNow,
            MerchantId = "MERCHANT123"
        };

        // Act
        var result = _guard.Validate(txn);

        // Assert
        Assert.Equal("Missing AccountId", result.Reason);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WithEmptyAccountId_ReturnsReject()
    {
        // Arrange
        var txn = new TransactionMessage
        {
            Id = 1,
            AccountId = string.Empty,
            Amount = 1000,
            Currency = "ZAR",
            Timestamp = DateTime.UtcNow,
            MerchantId = "MERCHANT123"
        };

        // Act
        var result = _guard.Validate(txn);

        // Assert
        Assert.Equal("Missing AccountId", result.Reason);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WithWhitespaceAccountId_ReturnsReject()
    {
        // Arrange
        var txn = new TransactionMessage
        {
            Id = 1,
            AccountId = "   ",
            Amount = 1000,
            Currency = "ZAR",
            Timestamp = DateTime.UtcNow,
            MerchantId = "MERCHANT123"
        };

        // Act
        var result = _guard.Validate(txn);

        // Assert
        Assert.Equal("Missing AccountId", result.Reason);
        Assert.False(result.IsValid);
    }

    #endregion

    #region Amount Validation Tests

    [Fact]
    public void Validate_WithZeroAmount_ReturnsReject()
    {
        // Arrange
        var txn = new TransactionMessage
        {
            Id = 1,
            AccountId = "ACC123",
            Amount = 0,
            Currency = "ZAR",
            Timestamp = DateTime.UtcNow,
            MerchantId = "MERCHANT123"
        };

        // Act
        var result = _guard.Validate(txn);

        // Assert
        Assert.Equal("Invalid amount", result.Reason);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WithNegativeAmount_ReturnsReject()
    {
        // Arrange
        var txn = new TransactionMessage
        {
            Id = 1,
            AccountId = "ACC123",
            Amount = -500,
            Currency = "ZAR",
            Timestamp = DateTime.UtcNow,
            MerchantId = "MERCHANT123"
        };

        // Act
        var result = _guard.Validate(txn);

        // Assert
        Assert.Equal("Invalid amount", result.Reason);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WithAmountExceedingSystemLimit_ReturnsQuarantine()
    {
        // Arrange
        var txn = new TransactionMessage
        {
            Id = 1,
            AccountId = "ACC123",
            Amount = 1_000_001,
            Currency = "ZAR",
            Timestamp = DateTime.UtcNow,
            MerchantId = "MERCHANT123"
        };

        // Act
        var result = _guard.Validate(txn);

        // Assert
        Assert.Equal("Amount exceeds system limit", result.Reason);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WithAmountEqualToSystemLimit_ReturnsPass()
    {
        // Arrange
        var txn = new TransactionMessage
        {
            Id = 1,
            AccountId = "ACC123",
            Amount = 1_000_000,
            Currency = "ZAR",
            Timestamp = DateTime.UtcNow,
            MerchantId = "MERCHANT123"
        };

        // Act
        var result = _guard.Validate(txn);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithHighValueTransaction_ReturnsReview()
    {
        // Arrange
        var txn = new TransactionMessage
        {
            Id = 1,
            AccountId = "ACC123",
            Amount = 300_000,
            Currency = "ZAR",
            Timestamp = DateTime.UtcNow,
            MerchantId = "MERCHANT123"
        };

        // Act
        var result = _guard.Validate(txn);

        // Assert
        Assert.Equal("High-value transaction", result.Reason);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WithAmountEqualToHighValueThreshold_ReturnsReview()
    {
        // Arrange
        var txn = new TransactionMessage
        {
            Id = 1,
            AccountId = "ACC123",
            Amount = 250_001,
            Currency = "ZAR",
            Timestamp = DateTime.UtcNow,
            MerchantId = "MERCHANT123"
        };

        // Act
        var result = _guard.Validate(txn);

        // Assert
        Assert.Equal("High-value transaction", result.Reason);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WithAmountJustBelowHighValueThreshold_ReturnsPass()
    {
        // Arrange
        var txn = new TransactionMessage
        {
            Id = 1,
            AccountId = "ACC123",
            Amount = 250_000,
            Currency = "ZAR",
            Timestamp = DateTime.UtcNow,
            MerchantId = "MERCHANT123"
        };

        // Act
        var result = _guard.Validate(txn);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithSmallValidAmount_ReturnsPass()
    {
        // Arrange
        var txn = new TransactionMessage
        {
            Id = 1,
            AccountId = "ACC123",
            Amount = 100,
            Currency = "ZAR",
            Timestamp = DateTime.UtcNow,
            MerchantId = "MERCHANT123"
        };

        // Act
        var result = _guard.Validate(txn);

        // Assert
        Assert.True(result.IsValid);
    }

    #endregion

    #region Currency Validation Tests

    [Fact]
    public void Validate_WithForeignCurrency_ReturnsReview()
    {
        // Arrange
        var txn = new TransactionMessage
        {
            Id = 1,
            AccountId = "ACC123",
            Amount = 1000,
            Currency = "USD",
            Timestamp = DateTime.UtcNow,
            MerchantId = "MERCHANT123"
        };

        // Act
        var result = _guard.Validate(txn);

        // Assert
        Assert.Equal("Foreign currency transaction", result.Reason);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WithZARCurrency_PassesCurrencyValidation()
    {
        // Arrange
        var txn = new TransactionMessage
        {
            Id = 1,
            AccountId = "ACC123",
            Amount = 1000,
            Currency = "ZAR",
            Timestamp = DateTime.UtcNow,
            MerchantId = "MERCHANT123"
        };

        // Act
        var result = _guard.Validate(txn);

        // Assert
        Assert.NotEqual("Foreign currency transaction", result.Reason);
    }

    [Fact]
    public void Validate_WithEURCurrency_ReturnsReview()
    {
        // Arrange
        var txn = new TransactionMessage
        {
            Id = 1,
            AccountId = "ACC123",
            Amount = 1000,
            Currency = "EUR",
            Timestamp = DateTime.UtcNow,
            MerchantId = "MERCHANT123"
        };

        // Act
        var result = _guard.Validate(txn);

        // Assert
        Assert.Equal("Foreign currency transaction", result.Reason);
    }

    #endregion

    #region Timestamp Validation Tests

    [Fact]
    public void Validate_WithFutureDateTransaction_ReturnsReject()
    {
        // Arrange
        var txn = new TransactionMessage
        {
            Id = 1,
            AccountId = "ACC123",
            Amount = 1000,
            Currency = "ZAR",
            Timestamp = DateTime.UtcNow.AddMinutes(10),
            MerchantId = "MERCHANT123"
        };

        // Act
        var result = _guard.Validate(txn);

        // Assert
        Assert.Equal("Future-dated transaction", result.Reason);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WithTimestampAtFutureLimit_ReturnsReject()
    {
        // Arrange
        var txn = new TransactionMessage
        {
            Id = 1,
            AccountId = "ACC123",
            Amount = 1000,
            Currency = "ZAR",
            Timestamp = DateTime.UtcNow.AddMinutes(5).AddSeconds(1),
            MerchantId = "MERCHANT123"
        };

        // Act
        var result = _guard.Validate(txn);

        // Assert
        Assert.Equal("Future-dated transaction", result.Reason);
    }

    [Fact]
    public void Validate_WithCurrentTimestamp_PassesTimestampValidation()
    {
        // Arrange
        var txn = new TransactionMessage
        {
            Id = 1,
            AccountId = "ACC123",
            Amount = 1000,
            Currency = "ZAR",
            Timestamp = DateTime.UtcNow,
            MerchantId = "MERCHANT123"
        };

        // Act
        var result = _guard.Validate(txn);

        // Assert
        Assert.NotEqual("Future-dated transaction", result.Reason);
    }

    [Fact]
    public void Validate_WithPastTimestamp_PassesTimestampValidation()
    {
        // Arrange
        var txn = new TransactionMessage
        {
            Id = 1,
            AccountId = "ACC123",
            Amount = 1000,
            Currency = "ZAR",
            Timestamp = DateTime.UtcNow.AddMinutes(-30),
            MerchantId = "MERCHANT123"
        };

        // Act
        var result = _guard.Validate(txn);

        // Assert
        Assert.NotEqual("Future-dated transaction", result.Reason);
    }

    #endregion

    #region Merchant Validation Tests

    [Fact]
    public void Validate_WithUnknownMerchant_ReturnsQuarantine()
    {
        // Arrange
        var txn = new TransactionMessage
        {
            Id = 1,
            AccountId = "ACC123",
            Amount = 1000,
            Currency = "ZAR",
            Timestamp = DateTime.UtcNow,
            MerchantId = "UNKNOWN"
        };

        // Act
        var result = _guard.Validate(txn);

        // Assert
        Assert.Equal("Unknown merchant", result.Reason);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WithMerchantContainingUnknown_ReturnsQuarantine()
    {
        // Arrange
        var txn = new TransactionMessage
        {
            Id = 1,
            AccountId = "ACC123",
            Amount = 1000,
            Currency = "ZAR",
            Timestamp = DateTime.UtcNow,
            MerchantId = "PREFIX_UNKNOWN_SUFFIX"
        };

        // Act
        var result = _guard.Validate(txn);

        // Assert
        Assert.Equal("Unknown merchant", result.Reason);
    }

    [Fact]
    public void Validate_WithKnownMerchant_PassesMerchantValidation()
    {
        // Arrange
        var txn = new TransactionMessage
        {
            Id = 1,
            AccountId = "ACC123",
            Amount = 1000,
            Currency = "ZAR",
            Timestamp = DateTime.UtcNow,
            MerchantId = "MERCHANT123"
        };

        // Act
        var result = _guard.Validate(txn);

        // Assert
        Assert.NotEqual("Unknown merchant", result.Reason);
    }

    #endregion

    #region Valid Transaction Tests

    [Fact]
    public void Validate_WithAllValidFields_ReturnsPass()
    {
        // Arrange
        var txn = new TransactionMessage
        {
            Id = 1,
            AccountId = "ACC123",
            Amount = 5000,
            Currency = "ZAR",
            Timestamp = DateTime.UtcNow.AddMinutes(-5),
            MerchantId = "MERCHANT456"
        };

        // Act
        var result = _guard.Validate(txn);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithMinimumValidAmount_ReturnsPass()
    {
        // Arrange
        var txn = new TransactionMessage
        {
            Id = 1,
            AccountId = "ACC123",
            Amount = 0.01m,
            Currency = "ZAR",
            Timestamp = DateTime.UtcNow,
            MerchantId = "MERCHANT789"
        };

        // Act
        var result = _guard.Validate(txn);

        // Assert
        Assert.True(result.IsValid);
    }

    #endregion
}