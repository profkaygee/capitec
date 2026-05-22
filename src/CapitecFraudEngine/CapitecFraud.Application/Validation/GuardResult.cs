namespace CapitecFraud.Application.Validation;

public class GuardResult
{
    public bool IsValid { get; set; }

    public GuardAction Action { get; set; }

    public string Reason { get; set; }

    public static GuardResult Pass()
        => new() { IsValid = true, Action = GuardAction.Allow };

    public static GuardResult Reject(string reason)
        => new() { IsValid = false, Action = GuardAction.Reject, Reason = reason };

    public static GuardResult Quarantine(string reason)
        => new() { IsValid = false, Action = GuardAction.Quarantine, Reason = reason };

    public static GuardResult Review(string reason)
        => new() { IsValid = false, Action = GuardAction.Review, Reason = reason };
}