using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parsalud.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class DynamicParams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Param1",
                table: "Sections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Param2",
                table: "Sections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Param3",
                table: "Sections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Param4",
                table: "Sections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Param5",
                table: "Sections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Param6",
                table: "Sections",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Param1",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "Param2",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "Param3",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "Param4",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "Param5",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "Param6",
                table: "Sections");
        }
    }
}
