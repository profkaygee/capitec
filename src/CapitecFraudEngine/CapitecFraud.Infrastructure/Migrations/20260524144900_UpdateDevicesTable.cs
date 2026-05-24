using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CapitecFraud.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDevicesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "Devices",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "Devices");
        }
    }
}
