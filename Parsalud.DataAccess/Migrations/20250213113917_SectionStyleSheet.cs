using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parsalud.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SectionStyleSheet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "StyleSheetId",
                table: "Sections",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sections_StyleSheetId",
                table: "Sections",
                column: "StyleSheetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sections_StyleSheets_StyleSheetId",
                table: "Sections",
                column: "StyleSheetId",
                principalTable: "StyleSheets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sections_StyleSheets_StyleSheetId",
                table: "Sections");

            migrationBuilder.DropIndex(
                name: "IX_Sections_StyleSheetId",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "StyleSheetId",
                table: "Sections");
        }
    }
}
