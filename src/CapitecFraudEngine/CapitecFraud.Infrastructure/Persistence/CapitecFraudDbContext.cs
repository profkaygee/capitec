using CapitecFraud.Domain.Entities;
using CapitecFraud.Domain.Rules;
using Microsoft.EntityFrameworkCore;

namespace CapitecFraud.Infrastructure.Persistence;

public class CapitecFraudDbContext : DbContext
{
    public CapitecFraudDbContext(DbContextOptions<CapitecFraudDbContext> options)
        : base(options)
    {
    }

    // =========================
    // CORE TABLES
    // =========================

    public DbSet<Transaction> Transactions => Set<Transaction>();

    public DbSet<FraudResult> FraudResults => Set<FraudResult>();

    public DbSet<FraudFlag> FraudFlags => Set<FraudFlag>();

    // =========================
    // DYNAMIC RULE ENGINE
    // =========================

    public DbSet<RuleConfig> FraudRules => Set<RuleConfig>();

    public DbSet<FraudDecisionRuleEntity> FraudDecisionRules => Set<FraudDecisionRuleEntity>();

    // =========================
    // AUDIT (COMPLIANCE)
    // =========================

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // =========================
        // TRANSACTION
        // =========================
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedOnAdd();
            entity.Property(x => x.Amount).HasPrecision(18, 2);
            entity.Property(x => x.Country).HasMaxLength(50);
        });

        // =========================
        // FRAUD RESULT
        // =========================
        modelBuilder.Entity<FraudResult>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedOnAdd();
            entity.OwnsMany(x => x.Flags);
        });

        // =========================
        // FRAUD FLAGS
        // =========================
        modelBuilder.Entity<FraudFlag>(entity =>
        {
            entity.HasKey("Id");
            entity.Property(x => x.Id).ValueGeneratedOnAdd();
        });

        // =========================
        // RULE CONFIG (DYNAMIC RULES)
        // =========================
        modelBuilder.Entity<RuleConfig>(entity =>
        {
            entity.HasKey(x => x.Name);
            entity.Property(x => x.Field).IsRequired();
            entity.Property(x => x.Operator).IsRequired();
        });

        // =========================
        // DECISION RULES
        // =========================
        modelBuilder.Entity<FraudDecisionRuleEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
        });

        // =========================
        // AUDIT LOG
        // =========================
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Action).HasMaxLength(100);
        });
    }
}