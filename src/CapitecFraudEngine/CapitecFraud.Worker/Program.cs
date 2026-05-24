using CapitecFraud.Application.Abstractions;
using CapitecFraud.Infrastructure.Extensions;
using CapitecFraud.Infrastructure.Persistence;
using CapitecFraud.Worker.Notifications;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Serilog;

namespace CapitecFraud.Worker;

public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithThreadId()
            .Enrich.WithProcessId()
            .WriteTo.Console()
            .WriteTo.File(
                path: "Logs/capitec-workers-fraud-logs-.txt",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 14,
                shared: true)
            .CreateLogger();

        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddHostedService<Worker>();
        
        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(Log.Logger, dispose: true);
        
        builder.Services.AddScoped<IRealtimeNotifier, NoOpNotifier>();
        builder.Services.AddFraudServices();

        // Register the database context
        builder.Services.AddDbContext<CapitecFraudDbContext>(options =>
        {
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("CapitecFraudDb"),
                sql =>
                {
                    sql.EnableRetryOnFailure();
                });
        });
        
        // Register RabbitMQ connection
        builder.Services.AddSingleton<IConnection>(sp =>
        {
            var factory = new ConnectionFactory()
            {
                HostName = "rabbitmq",
                UserName = "guest",
                Password = "guest",
                DispatchConsumersAsync = true
            };
            
            return factory.CreateConnection();
        });

        var host = builder.Build();
        host.Run();
    }
}