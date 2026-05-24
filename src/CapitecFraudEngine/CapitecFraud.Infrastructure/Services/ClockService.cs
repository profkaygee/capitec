using CapitecFraud.Domain.Abstractions.Services;

namespace CapitecFraud.Infrastructure.Services;

public class ClockService:IClockService
{
    public DateTime UtcNow { get; set; }
}