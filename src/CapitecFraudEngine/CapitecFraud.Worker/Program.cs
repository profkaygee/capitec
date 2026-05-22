using CapitecFraud.Application.Abstractions;
using CapitecFraud.Domain.Abstractions.Repositories;
using CapitecFraud.Domain.Abstractions.Services;
using CapitecFraud.Infrastructure.Extensions;
using CapitecFraud.Infrastructure.Messaging;
using CapitecFraud.Infrastructure.Persistence;
using CapitecFraud.Infrastructure.Realtime;
using CapitecFraud.Infrastructure.Repositories;
using CapitecFraud.Infrastructure.Services;
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
            .WriteTo.Console()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Service", "FraudWorker")
            .CreateLogger();

        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddHostedService<Worker>();
        
        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(Log.Logger, dispose: true);

        builder.Services.AddScoped<IAuditService, AuditService>();
        builder.Services.AddScoped<ITransactionQueue, RabbitMqTransactionQueue>();
        builder.Services.AddScoped<IRuleRepository, RuleRepository>();
        builder.Services.AddScoped<IFraudResultRepository, FraudResultRepository>();
        builder.Services.AddScoped<IAuditService, AuditService>();
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