using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GolBet.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class Modulo9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PrimaryColor",
                table: "Teams",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrimaryColor",
                table: "Teams");
        }
    }
}
