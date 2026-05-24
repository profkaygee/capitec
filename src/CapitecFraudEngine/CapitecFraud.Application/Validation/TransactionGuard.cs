using CapitecFraud.Domain.Models;

namespace CapitecFraud.Application.Validation;

public class TransactionGuard
{
    public GuardResult Validate(TransactionMessage txn)
    {
        if (txn == null)
            return GuardResult.Reject("Transaction is null");

        if (string.IsNullOrWhiteSpace(txn.AccountId))
            return GuardResult.Reject("Missing AccountId");

        if (txn.Amount <= 0)
            return GuardResult.Reject("Invalid amount");

        if (txn.Amount > 1_000_000)
            return GuardResult.Quarantine("Amount exceeds system limit");

        if (txn.Currency != "ZAR" && txn.Currency != "USD" && txn.Currency != "EUR" && txn.Currency != "GBP")
            return GuardResult.Review("Foreign currency transaction");

        if (txn.Timestamp > DateTime.UtcNow.AddMinutes(5))
            return GuardResult.Reject("Future-dated transaction");

        if (txn.Amount > 250_000)
            return GuardResult.Review("High-value transaction");

        return txn.MerchantId.Contains("UNKNOWN") 
            ? GuardResult.Quarantine("Unknown merchant") 
            : GuardResult.Pass();
    }
}