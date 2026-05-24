using CapitecFraud.Domain.Entities;
using CapitecFraud.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CapitecFraud.Infrastructure.Persistence;

public class CapitecFraudDbContext(DbContextOptions<CapitecFraudDbContext> options)
    : DbContext(options)
{
    public DbSet<RuleConfig> Rules => Set<RuleConfig>();
    public DbSet<FraudResult> FraudResults => Set<FraudResult>();
    public DbSet<FraudFlag> FraudFlags => Set<FraudFlag>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<RuleConfig> RuleConfigs => Set<RuleConfig>();
    public DbSet<FraudDecisionRuleConfig> FraudDecisionRuleConfigs => Set<FraudDecisionRuleConfig>();
    public DbSet<TransactionMessage> Transactions => Set<TransactionMessage>();
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<AccountDevice> AccountDevices => Set<AccountDevice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<FraudResult>()
            .HasMany(r => r.Flags)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FraudResult>()
            .Property(x => x.Decision)
            .HasConversion<string>();

        modelBuilder.Entity<TransactionMessage>()
            .Property(t => t.Amount)
            .HasPrecision(18, 2);

        // Seed rules configurations data
        modelBuilder.Entity<FraudDecisionRuleConfig>().HasData(
            new FraudDecisionRuleConfig
            {
                Id = 1,
                Name = "Large Amount Anomaly",
                ReviewThreshold = 60,
                BlockThreshold = 85
            },
            new FraudDecisionRuleConfig
            {
                Id = 2,
                Name = "High Velocity Transactions",
                ReviewThreshold = 55,
                BlockThreshold = 80
            },
            new FraudDecisionRuleConfig
            {
                Id = 3,
                Name = "Structuring (Just Below Threshold Pattern)",
                ReviewThreshold = 65,
                BlockThreshold = 90
            },
            new FraudDecisionRuleConfig
            {
                Id = 4,
                Name = "Impossible Travel (Geo Velocity)",
                ReviewThreshold = 50,
                BlockThreshold = 75
            },
            new FraudDecisionRuleConfig
            {
                Id = 5,
                Name = "New Device + High Value Transaction",
                ReviewThreshold = 60,
                BlockThreshold = 85
            },
            new FraudDecisionRuleConfig
            {
                Id = 6,
                Name = "VPN / Proxy Detected",
                ReviewThreshold = 45,
                BlockThreshold = 70
            },
            new FraudDecisionRuleConfig
            {
                Id = 7,
                Name = "Unusual Country Risk",
                ReviewThreshold = 55,
                BlockThreshold = 80
            },
            new FraudDecisionRuleConfig
            {
                Id = 8,
                Name = "Password Reset Followed by Transaction",
                ReviewThreshold = 65,
                BlockThreshold = 90
            },
            new FraudDecisionRuleConfig
            {
                Id = 9,
                Name = "Multiple Failed Login Attempts",
                ReviewThreshold = 50,
                BlockThreshold = 75
            },
            new FraudDecisionRuleConfig
            {
                Id = 10,
                Name = "SIM Swap Indicator Detected",
                ReviewThreshold = 70,
                BlockThreshold = 95
            },
            new FraudDecisionRuleConfig
            {
                Id = 11,
                Name = "First-Time Beneficiary Large Transfer",
                ReviewThreshold = 60,
                BlockThreshold = 85
            },
            new FraudDecisionRuleConfig
            {
                Id = 12,
                Name = "High-Risk Merchant Category",
                ReviewThreshold = 55,
                BlockThreshold = 80
            },
            new FraudDecisionRuleConfig
            {
                Id = 13,
                Name = "Card-Not-Present High Value Transaction",
                ReviewThreshold = 50,
                BlockThreshold = 78
            },
            new FraudDecisionRuleConfig
            {
                Id = 14,
                Name = "Shared Device Across Multiple Accounts",
                ReviewThreshold = 65,
                BlockThreshold = 88
            },
            new FraudDecisionRuleConfig
            {
                Id = 15,
                Name = "Circular Money Flow Detected",
                ReviewThreshold = 70,
                BlockThreshold = 95
            },
            new FraudDecisionRuleConfig
            {
                Id = 16,
                Name = "Multiple Accounts Same IP Address",
                ReviewThreshold = 55,
                BlockThreshold = 80
            },
            new FraudDecisionRuleConfig
            {
                Id = 17,
                Name = "Sudden Spending Behavior Change",
                ReviewThreshold = 50,
                BlockThreshold = 75
            },
            new FraudDecisionRuleConfig
            {
                Id = 18,
                Name = "Dormant Account Sudden Activity",
                ReviewThreshold = 60,
                BlockThreshold = 85
            },
            new FraudDecisionRuleConfig
            {
                Id = 19,
                Name = "Abnormal Transaction Time Pattern",
                ReviewThreshold = 45,
                BlockThreshold = 70
            },
            new FraudDecisionRuleConfig
            {
                Id = 20,
                Name = "High Risk Combination (New Device + New Payee + High Amount)",
                ReviewThreshold = 40,
                BlockThreshold = 65
            },
            new FraudDecisionRuleConfig
            {
                Id = 21,
                Name = "Account Takeover Pattern Detected",
                ReviewThreshold = 70,
                BlockThreshold = 95
            }
        );

        // Seed the rules data
        modelBuilder.Entity<RuleConfig>().HasData(
            new RuleConfig
            {
                Id = 1,
                Name = "Large Transaction Amount",
                Field = "Amount",
                Operator = ">",
                Value = "20000",
                ActionWeight = 30,
                IsActive = true
            },
            new RuleConfig
            {
                Id = 2,
                Name = "Very Large Transaction Amount",
                Field = "Amount",
                Operator = ">",
                Value = "50000",
                ActionWeight = 50,
                IsActive = true
            },
            new RuleConfig
            {
                Id = 3,
                Name = "High Velocity Transactions",
                Field = "TransactionCountLast10Min",
                Operator = ">",
                Value = "5",
                ActionWeight = 35,
                IsActive = true
            },
            new RuleConfig
            {
                Id = 4,
                Name = "Excessive Velocity Transactions",
                Field = "TransactionCountLast1Min",
                Operator = ">",
                Value = "3",
                ActionWeight = 45,
                IsActive = true
            },
            new RuleConfig
            {
                Id = 5,
                Name = "Country Change Anomaly",
                Field = "CountryChangeLast24H",
                Operator = ">",
                Value = "1",
                ActionWeight = 40,
                IsActive = true
            },
            new RuleConfig
            {
                Id = 6,
                Name = "Impossible Travel Detected",
                Field = "GeoDistanceKmPerHour",
                Operator = ">",
                Value = "900",
                ActionWeight = 60,
                IsActive = true
            },
            new RuleConfig
            {
                Id = 7,
                Name = "High Risk Country",
                Field = "CountryRiskScore",
                Operator = ">",
                Value = "70",
                ActionWeight = 35,
                IsActive = true
            },
            new RuleConfig
            {
                Id = 8,
                Name = "New Device Usage",
                Field = "IsNewDevice",
                Operator = "=",
                Value = "true",
                ActionWeight = 25,
                IsActive = true
            },
            new RuleConfig
            {
                Id = 9,
                Name = "Device Shared Across Multiple Accounts",
                Field = "DeviceAccountCount",
                Operator = ">",
                Value = "3",
                ActionWeight = 50,
                IsActive = true
            },
            new RuleConfig
            {
                Id = 10,
                Name = "Suspicious Device Fingerprint",
                Field = "DeviceRiskScore",
                Operator = ">",
                Value = "80",
                ActionWeight = 55,
                IsActive = true
            },
            new RuleConfig
            {
                Id = 11,
                Name = "Password Reset Before Transaction",
                Field = "PasswordResetLastMinutes",
                Operator = "<",
                Value = "30",
                ActionWeight = 50,
                IsActive = true
            },
            new RuleConfig
            {
                Id = 12,
                Name = "Multiple Failed Login Attempts",
                Field = "FailedLoginAttempts",
                Operator = ">",
                Value = "3",
                ActionWeight = 35,
                IsActive = true
            },
            new RuleConfig
            {
                Id = 13,
                Name = "OTP Resend Abuse",
                Field = "OtpResendCount",
                Operator = ">",
                Value = "5",
                ActionWeight = 30,
                IsActive = true
            },
            new RuleConfig
            {
                Id = 14,
                Name = "First Time Beneficiary",
                Field = "IsNewBeneficiary",
                Operator = "=",
                Value = "true",
                ActionWeight = 25,
                IsActive = true
            },
            new RuleConfig
            {
                Id = 15,
                Name = "High Risk Merchant Category",
                Field = "MerchantCategory",
                Operator = "IN",
                Value = "CRYPTO,GAMBLING,FOREX,GIFT_CARDS",
                ActionWeight = 45,
                IsActive = true
            },
            new RuleConfig
            {
                Id = 16,
                Name = "Card Not Present High Value",
                Field = "TransactionChannel",
                Operator = "=",
                Value = "CNP",
                ActionWeight = 20,
                IsActive = true
            },
            new RuleConfig
            {
                Id = 17,
                Name = "Shared IP Across Accounts",
                Field = "IpAccountCount",
                Operator = ">",
                Value = "5",
                ActionWeight = 40,
                IsActive = true
            },
            new RuleConfig
            {
                Id = 18,
                Name = "Shared Device Across Accounts",
                Field = "DeviceAccountCount",
                Operator = ">",
                Value = "3",
                ActionWeight = 45,
                IsActive = true
            },
            new RuleConfig
            {
                Id = 19,
                Name = "Circular Money Flow Detected",
                Field = "CircularFlowDetected",
                Operator = "=",
                Value = "true",
                ActionWeight = 70,
                IsActive = true
            },
            new RuleConfig
            {
                Id = 20,
                Name = "Dormant Account Reactivation",
                Field = "DaysSinceLastTransaction",
                Operator = ">",
                Value = "90",
                ActionWeight = 30,
                IsActive = true
            },
            new RuleConfig
            {
                Id = 21,
                Name = "Sudden Spending Spike",
                Field = "SpendingDeviationPercent",
                Operator = ">",
                Value = "300",
                ActionWeight = 35,
                IsActive = true
            },
            new RuleConfig
            {
                Id = 22,
                Name = "Unusual Transaction Time",
                Field = "IsUnusualHour",
                Operator = "=",
                Value = "true",
                ActionWeight = 15,
                IsActive = true
            },
            new RuleConfig
            {
                Id = 23,
                Name = "New Device + New Beneficiary + High Amount",
                Field = "CompositeRisk",
                Operator = "=",
                Value = "TRUE",
                ActionWeight = 75,
                IsActive = true
            },
            new RuleConfig
            {
                Id = 24,
                Name = "Multiple High Risk Signals Detected",
                Field = "RiskSignalCount",
                Operator = ">",
                Value = "3",
                ActionWeight = 80,
                IsActive = true
            }
        );
    }
}