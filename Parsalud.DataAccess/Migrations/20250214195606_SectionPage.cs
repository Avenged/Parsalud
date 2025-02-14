using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parsalud.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SectionPage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Page",
                table: "Sections",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Page",
                table: "Sections");
        }
    }
}
