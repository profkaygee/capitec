using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CapitecFraud.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFutherEnrichmentsToDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Fingerprint = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstSeen = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastSeen = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MerchantId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccountDevices",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeviceId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountDevices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountDevices_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "RuleConfig",
                columns: new[] { "Id", "ActionWeight", "Field", "IsActive", "Name", "Operator", "Value" },
                values: new object[,]
                {
                    { 1L, 30, "Amount", true, "Large Transaction Amount", ">", "20000" },
                    { 2L, 50, "Amount", true, "Very Large Transaction Amount", ">", "50000" },
                    { 3L, 35, "TransactionCountLast10Min", true, "High Velocity Transactions", ">", "5" },
                    { 4L, 45, "TransactionCountLast1Min", true, "Excessive Velocity Transactions", ">", "3" },
                    { 5L, 40, "CountryChangeLast24H", true, "Country Change Anomaly", ">", "1" },
                    { 6L, 60, "GeoDistanceKmPerHour", true, "Impossible Travel Detected", ">", "900" },
                    { 7L, 35, "CountryRiskScore", true, "High Risk Country", ">", "70" },
                    { 8L, 25, "IsNewDevice", true, "New Device Usage", "=", "true" },
                    { 9L, 50, "DeviceAccountCount", true, "Device Shared Across Multiple Accounts", ">", "3" },
                    { 10L, 55, "DeviceRiskScore", true, "Suspicious Device Fingerprint", ">", "80" },
                    { 11L, 50, "PasswordResetLastMinutes", true, "Password Reset Before Transaction", "<", "30" },
                    { 12L, 35, "FailedLoginAttempts", true, "Multiple Failed Login Attempts", ">", "3" },
                    { 13L, 30, "OtpResendCount", true, "OTP Resend Abuse", ">", "5" },
                    { 14L, 25, "IsNewBeneficiary", true, "First Time Beneficiary", "=", "true" },
                    { 15L, 45, "MerchantCategory", true, "High Risk Merchant Category", "IN", "CRYPTO,GAMBLING,FOREX,GIFT_CARDS" },
                    { 16L, 20, "TransactionChannel", true, "Card Not Present High Value", "=", "CNP" },
                    { 17L, 40, "IpAccountCount", true, "Shared IP Across Accounts", ">", "5" },
                    { 18L, 45, "DeviceAccountCount", true, "Shared Device Across Accounts", ">", "3" },
                    { 19L, 70, "CircularFlowDetected", true, "Circular Money Flow Detected", "=", "true" },
                    { 20L, 30, "DaysSinceLastTransaction", true, "Dormant Account Reactivation", ">", "90" },
                    { 21L, 35, "SpendingDeviationPercent", true, "Sudden Spending Spike", ">", "300" },
                    { 22L, 15, "IsUnusualHour", true, "Unusual Transaction Time", "=", "true" },
                    { 23L, 75, "CompositeRisk", true, "New Device + New Beneficiary + High Amount", "=", "TRUE" },
                    { 24L, 80, "RiskSignalCount", true, "Multiple High Risk Signals Detected", ">", "3" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountDevices_DeviceId",
                table: "AccountDevices",
                column: "DeviceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountDevices");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DeleteData(
                table: "RuleConfig",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "RuleConfig",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "RuleConfig",
                keyColumn: "Id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "RuleConfig",
                keyColumn: "Id",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "RuleConfig",
                keyColumn: "Id",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                table: "RuleConfig",
                keyColumn: "Id",
                keyValue: 6L);

            migrationBuilder.DeleteData(
                table: "RuleConfig",
                keyColumn: "Id",
                keyValue: 7L);

            migrationBuilder.DeleteData(
                table: "RuleConfig",
                keyColumn: "Id",
                keyValue: 8L);

            migrationBuilder.DeleteData(
                table: "RuleConfig",
                keyColumn: "Id",
                keyValue: 9L);

            migrationBuilder.DeleteData(
                table: "RuleConfig",
                keyColumn: "Id",
                keyValue: 10L);

            migrationBuilder.DeleteData(
                table: "RuleConfig",
                keyColumn: "Id",
                keyValue: 11L);

            migrationBuilder.DeleteData(
                table: "RuleConfig",
                keyColumn: "Id",
                keyValue: 12L);

            migrationBuilder.DeleteData(
                table: "RuleConfig",
                keyColumn: "Id",
                keyValue: 13L);

            migrationBuilder.DeleteData(
                table: "RuleConfig",
                keyColumn: "Id",
                keyValue: 14L);

            migrationBuilder.DeleteData(
                table: "RuleConfig",
                keyColumn: "Id",
                keyValue: 15L);

            migrationBuilder.DeleteData(
                table: "RuleConfig",
                keyColumn: "Id",
                keyValue: 16L);

            migrationBuilder.DeleteData(
                table: "RuleConfig",
                keyColumn: "Id",
                keyValue: 17L);

            migrationBuilder.DeleteData(
                table: "RuleConfig",
                keyColumn: "Id",
                keyValue: 18L);

            migrationBuilder.DeleteData(
                table: "RuleConfig",
                keyColumn: "Id",
                keyValue: 19L);

            migrationBuilder.DeleteData(
                table: "RuleConfig",
                keyColumn: "Id",
                keyValue: 20L);

            migrationBuilder.DeleteData(
                table: "RuleConfig",
                keyColumn: "Id",
                keyValue: 21L);

            migrationBuilder.DeleteData(
                table: "RuleConfig",
                keyColumn: "Id",
                keyValue: 22L);

            migrationBuilder.DeleteData(
                table: "RuleConfig",
                keyColumn: "Id",
                keyValue: 23L);

            migrationBuilder.DeleteData(
                table: "RuleConfig",
                keyColumn: "Id",
                keyValue: 24L);
        }
    }
}
