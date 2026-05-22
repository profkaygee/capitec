using CapitecFraud.Application.Models;

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

        if (txn.Currency != "ZAR")
            return GuardResult.Review("Foreign currency transaction");

        if (txn.Timestamp > DateTime.UtcNow.AddMinutes(5))
            return GuardResult.Reject("Future-dated transaction");

        if (txn.Amount > 250_000)
            return GuardResult.Review("High-value transaction");

        if (txn.MerchantId.Contains("UNKNOWN"))
            return GuardResult.Quarantine("Unknown merchant");

        return GuardResult.Pass();
    }
}