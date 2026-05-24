namespace CapitecFraud.Domain.Abstractions.Services;

public interface IClockService
{
    DateTime UtcNow { get; }
}