using CapitecFraud.Api.Hubs;
using CapitecFraud.Application.Abstractions;
using CapitecFraud.Infrastructure.Persistence;
using CapitecFraud.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace CapitecFraud.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Services.AddScoped<IAuditService, AuditService>();
        
        // Add signal R
        builder.Services.AddSignalR();
        
        // Register the database context
        builder.Services.AddDbContext<CapitecFraudDbContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("CapitecFraudDb")));
        
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();

        // Add fraud notification hub
        app.MapHub<FraudNotificationHub>("/hubs/fraud");

        app.Run();
    }
}