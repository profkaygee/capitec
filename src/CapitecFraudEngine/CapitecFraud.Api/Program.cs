using CapitecFraud.Api.Endpoints;
using CapitecFraud.Api.Middleware;
using CapitecFraud.Application.Abstractions;
using CapitecFraud.Application.Common.Responses;
using CapitecFraud.Domain.Abstractions.Repositories;
using CapitecFraud.Domain.Abstractions.Services;
using CapitecFraud.Infrastructure.Extensions;
using CapitecFraud.Infrastructure.Messaging;
using CapitecFraud.Infrastructure.Persistence;
using CapitecFraud.Infrastructure.Realtime;
using CapitecFraud.Infrastructure.Repositories;
using CapitecFraud.Infrastructure.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Scalar.AspNetCore;
using Serilog;

namespace CapitecFraud.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAngularApp", policy =>
            {
                policy
                    .WithOrigins("http://localhost:4200", "http://localhost:8080") // Angular dev URL - To replace with production
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
        
        // ----------------------------
        // SERILOG CONFIGURATION
        // ----------------------------
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithThreadId()
            .Enrich.WithProcessId()
            .WriteTo.Console()
            .WriteTo.File(
                path: "Logs/capitec-fraud-logs-.txt",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 14,
                shared: true)
            .CreateLogger();

        builder.Host.UseSerilog();

        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Services.AddScoped<IAuditService, AuditService>();
        
        // Add signal R
        builder.Services.AddSignalR();
        
        // Register the database context
        builder.Services.AddDbContext<CapitecFraudDbContext>(options =>
        {
            Console.WriteLine("FINAL CONNECTION STRING >>> " +
                builder.Configuration.GetConnectionString("CapitecFraudDb"));
            options.UseSqlServer(builder.Configuration.GetConnectionString("CapitecFraudDb"),
                sqlServerOptionsAction: sqlOptions => sqlOptions.EnableRetryOnFailure());
        });
        
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddOpenApi();
        
        builder.Services.AddSingleton<IConnection>(sp =>
        {
            var factory = new ConnectionFactory()
            {
                HostName = "rabbitmq", // matches docker-compose service name
                DispatchConsumersAsync = true
            };

            return factory.CreateConnection();
        });

        builder.Services.AddScoped<ITransactionQueue, RabbitMqTransactionQueue>();
        builder.Services.AddScoped<IRuleRepository, RuleRepository>();
        builder.Services.AddFraudServices();

        var app = builder.Build();
        
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseMiddleware<RequestResponseLoggingMiddleware>();
        
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

                context.Response.ContentType = "application/json";

                switch (exception)
                {
                    case KeyNotFoundException ex:
                        context.Response.StatusCode = 404;
                        await context.Response.WriteAsJsonAsync(
                            ApiResponse<string>.ErrorResponse(ex.Message, 404)
                        );
                        break;

                    case ArgumentException ex:
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsJsonAsync(
                            ApiResponse<string>.ErrorResponse(ex.Message, 400)
                        );
                        break;

                    default:
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsJsonAsync(
                            ApiResponse<string>.ErrorResponse("Server error", 500)
                        );
                        break;
                }
            });
        });
        
        // Scalar UI
        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options.WithTitle("Capitec Fraud Backend API");
        });

        app.UseCors("AllowAngularApp");

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.UseSerilogRequestLogging(); // logs HTTP requests automatically

        // Add fraud notification hub
        app.MapHub<FraudNotificationHub>("/hubs/fraud");
        
        // Moved the endpoint to their respective endpoint's folder. Create a new class if you have a specific endpoint to create.
        app.ConfigureEndpoints("1");

        app.Run();
    }
}