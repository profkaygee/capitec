using CapitecFraud.Application.Abstractions;
using CapitecFraud.Infrastructure.Services;

namespace CapitecFraud.Worker;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddHostedService<Worker>();

        builder.Services.AddScoped<IAuditService, AuditService>();

        var host = builder.Build();
        host.Run();
    }
}