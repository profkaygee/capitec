namespace CapitecFraud.Domain.Enums;

public enum FraudDecision
{
    ALLOW = 0,
    REVIEW = 1,
    BLOCK = 2,
    REJECT = 3,
    QUARANTINE = 4
}