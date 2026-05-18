using CapitecFraud.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CapitecFraud.Infrastructure.Persistence;

public class CapitecFraudDbContext : DbContext
{
    public CapitecFraudDbContext(DbContextOptions<CapitecFraudDbContext> options)
        : base(options)
    {
    }

    public DbSet<RuleConfig> Rules => Set<RuleConfig>();
    public DbSet<FraudResult> FraudResults => Set<FraudResult>();
    public DbSet<FraudFlag> FraudFlags => Set<FraudFlag>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FraudResult>()
            .HasMany(r => r.Flags)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<FraudResult>()
            .Property(x => x.Decision)
            .HasConversion<string>();
    }
}