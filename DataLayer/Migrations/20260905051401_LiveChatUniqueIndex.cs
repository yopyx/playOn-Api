using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class LiveChatUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LiveComments_CId",
                table: "LiveComments");

            migrationBuilder.CreateIndex(
                name: "IX_LiveComments_CId_PageNum",
                table: "LiveComments",
                columns: new[] { "CId", "PageNum" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LiveComments_CId_PageNum",
                table: "LiveComments");

            migrationBuilder.CreateIndex(
                name: "IX_LiveComments_CId",
                table: "LiveComments",
                column: "CId",
                unique: true);
        }
    }
}
