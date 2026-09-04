using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebcamStudio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSiteTokenValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TokenValueUsd",
                table: "Sites",
                type: "decimal(10,4)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TokenValueUsd",
                table: "Sites");
        }
    }
}
