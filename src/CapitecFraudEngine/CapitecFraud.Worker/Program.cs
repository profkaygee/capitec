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
        
        // Register the database context
        // builder.Services.AddDbContext<CapitecFraudDbContext>(options =>
        // {
        //     options.UseSqlServer(builder.Configuration.GetConnectionString("CapitecFraudDb"),
        //         sqlServerOptionsAction: sqlOptions => sqlOptions.EnableRetryOnFailure());
        // });

        var host = builder.Build();
        host.Run();
    }
}