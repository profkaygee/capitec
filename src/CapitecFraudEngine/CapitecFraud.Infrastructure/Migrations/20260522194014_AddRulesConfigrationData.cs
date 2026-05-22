using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CapitecFraud.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRulesConfigrationData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "FraudDecisionRuleConfigs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "FraudDecisionRuleConfigs",
                columns: new[] { "Id", "BlockThreshold", "Name", "ReviewThreshold" },
                values: new object[,]
                {
                    { 1L, 85, "Large Amount Anomaly", 60 },
                    { 2L, 80, "High Velocity Transactions", 55 },
                    { 3L, 90, "Structuring (Just Below Threshold Pattern)", 65 },
                    { 4L, 75, "Impossible Travel (Geo Velocity)", 50 },
                    { 5L, 85, "New Device + High Value Transaction", 60 },
                    { 6L, 70, "VPN / Proxy Detected", 45 },
                    { 7L, 80, "Unusual Country Risk", 55 },
                    { 8L, 90, "Password Reset Followed by Transaction", 65 },
                    { 9L, 75, "Multiple Failed Login Attempts", 50 },
                    { 10L, 95, "SIM Swap Indicator Detected", 70 },
                    { 11L, 85, "First-Time Beneficiary Large Transfer", 60 },
                    { 12L, 80, "High-Risk Merchant Category", 55 },
                    { 13L, 78, "Card-Not-Present High Value Transaction", 50 },
                    { 14L, 88, "Shared Device Across Multiple Accounts", 65 },
                    { 15L, 95, "Circular Money Flow Detected", 70 },
                    { 16L, 80, "Multiple Accounts Same IP Address", 55 },
                    { 17L, 75, "Sudden Spending Behavior Change", 50 },
                    { 18L, 85, "Dormant Account Sudden Activity", 60 },
                    { 19L, 70, "Abnormal Transaction Time Pattern", 45 },
                    { 20L, 65, "High Risk Combination (New Device + New Payee + High Amount)", 40 },
                    { 21L, 95, "Account Takeover Pattern Detected", 70 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "FraudDecisionRuleConfigs",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "FraudDecisionRuleConfigs",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "FraudDecisionRuleConfigs",
                keyColumn: "Id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "FraudDecisionRuleConfigs",
                keyColumn: "Id",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "FraudDecisionRuleConfigs",
                keyColumn: "Id",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                table: "FraudDecisionRuleConfigs",
                keyColumn: "Id",
                keyValue: 6L);

            migrationBuilder.DeleteData(
                table: "FraudDecisionRuleConfigs",
                keyColumn: "Id",
                keyValue: 7L);

            migrationBuilder.DeleteData(
                table: "FraudDecisionRuleConfigs",
                keyColumn: "Id",
                keyValue: 8L);

            migrationBuilder.DeleteData(
                table: "FraudDecisionRuleConfigs",
                keyColumn: "Id",
                keyValue: 9L);

            migrationBuilder.DeleteData(
                table: "FraudDecisionRuleConfigs",
                keyColumn: "Id",
                keyValue: 10L);

            migrationBuilder.DeleteData(
                table: "FraudDecisionRuleConfigs",
                keyColumn: "Id",
                keyValue: 11L);

            migrationBuilder.DeleteData(
                table: "FraudDecisionRuleConfigs",
                keyColumn: "Id",
                keyValue: 12L);

            migrationBuilder.DeleteData(
                table: "FraudDecisionRuleConfigs",
                keyColumn: "Id",
                keyValue: 13L);

            migrationBuilder.DeleteData(
                table: "FraudDecisionRuleConfigs",
                keyColumn: "Id",
                keyValue: 14L);

            migrationBuilder.DeleteData(
                table: "FraudDecisionRuleConfigs",
                keyColumn: "Id",
                keyValue: 15L);

            migrationBuilder.DeleteData(
                table: "FraudDecisionRuleConfigs",
                keyColumn: "Id",
                keyValue: 16L);

            migrationBuilder.DeleteData(
                table: "FraudDecisionRuleConfigs",
                keyColumn: "Id",
                keyValue: 17L);

            migrationBuilder.DeleteData(
                table: "FraudDecisionRuleConfigs",
                keyColumn: "Id",
                keyValue: 18L);

            migrationBuilder.DeleteData(
                table: "FraudDecisionRuleConfigs",
                keyColumn: "Id",
                keyValue: 19L);

            migrationBuilder.DeleteData(
                table: "FraudDecisionRuleConfigs",
                keyColumn: "Id",
                keyValue: 20L);

            migrationBuilder.DeleteData(
                table: "FraudDecisionRuleConfigs",
                keyColumn: "Id",
                keyValue: 21L);

            migrationBuilder.DropColumn(
                name: "Name",
                table: "FraudDecisionRuleConfigs");
        }
    }
}
