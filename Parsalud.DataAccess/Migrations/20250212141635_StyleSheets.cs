using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parsalud.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class StyleSheets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StyleSheets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StyleSheets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StyleSheets_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StyleSheets_AspNetUsers_UpatedById",
                        column: x => x.UpatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_StyleSheets_CreatedById",
                table: "StyleSheets",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_StyleSheets_FileName",
                table: "StyleSheets",
                column: "FileName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StyleSheets_UpatedById",
                table: "StyleSheets",
                column: "UpatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StyleSheets");
        }
    }
}
