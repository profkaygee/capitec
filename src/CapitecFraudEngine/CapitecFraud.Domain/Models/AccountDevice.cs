namespace CapitecFraud.Domain.Models;

public class AccountDevice
{
    public long Id { get; set; }

    public string AccountId { get; set; }

    public string DeviceId { get; set; }

    public Device Device { get; set; }
}