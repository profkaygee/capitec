using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CapitecFraud.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRulesConfigurationsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Rules",
                table: "Rules");

            migrationBuilder.RenameTable(
                name: "Rules",
                newName: "RuleConfig");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RuleConfig",
                table: "RuleConfig",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "FraudDecisionRuleConfigs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReviewThreshold = table.Column<int>(type: "int", nullable: false),
                    BlockThreshold = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FraudDecisionRuleConfigs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FraudDecisionRuleConfigs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RuleConfig",
                table: "RuleConfig");

            migrationBuilder.RenameTable(
                name: "RuleConfig",
                newName: "Rules");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rules",
                table: "Rules",
                column: "Id");
        }
    }
}
