namespace CapitecFraud.Domain.Models;

public class Device
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Fingerprint { get; set; } = string.Empty;
    
    public string IpAddress { get; set; } = string.Empty;

    public DateTime FirstSeen { get; set; } = DateTime.UtcNow;

    public DateTime LastSeen { get; set; } = DateTime.UtcNow;

    public ICollection<AccountDevice> AccountDevices { get; set; } = new List<AccountDevice>();
}