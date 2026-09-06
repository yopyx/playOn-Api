using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddPageNumProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Videos_VId_NextToken",
                table: "Videos");

            migrationBuilder.DropIndex(
                name: "IX_VideoResults_VId_NextToken",
                table: "VideoResults");

            migrationBuilder.DropIndex(
                name: "IX_History_UserId",
                table: "History");

            migrationBuilder.DropColumn(
                name: "VideoListEtag",
                table: "SearchKeys");

            migrationBuilder.DropColumn(
                name: "PageNo",
                table: "LiveComments");

            migrationBuilder.AddColumn<int>(
                name: "PageNum",
                table: "Videos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PageNum",
                table: "VideoResults",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PageNum",
                table: "VideoCategories",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PageNum",
                table: "SearchKeys",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PageNum",
                table: "LiveComments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Videos_VId_PageNum",
                table: "Videos",
                columns: new[] { "VId", "PageNum" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VideoResults_VId_PageNum",
                table: "VideoResults",
                columns: new[] { "VId", "PageNum" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_History_UserId",
                table: "History",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Videos_VId_PageNum",
                table: "Videos");

            migrationBuilder.DropIndex(
                name: "IX_VideoResults_VId_PageNum",
                table: "VideoResults");

            migrationBuilder.DropIndex(
                name: "IX_History_UserId",
                table: "History");

            migrationBuilder.DropColumn(
                name: "PageNum",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "PageNum",
                table: "VideoResults");

            migrationBuilder.DropColumn(
                name: "PageNum",
                table: "VideoCategories");

            migrationBuilder.DropColumn(
                name: "PageNum",
                table: "SearchKeys");

            migrationBuilder.DropColumn(
                name: "PageNum",
                table: "LiveComments");

            migrationBuilder.AddColumn<string>(
                name: "VideoListEtag",
                table: "SearchKeys",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PageNo",
                table: "LiveComments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_VId_NextToken",
                table: "Videos",
                columns: new[] { "VId", "NextToken" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VideoResults_VId_NextToken",
                table: "VideoResults",
                columns: new[] { "VId", "NextToken" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_History_UserId",
                table: "History",
                column: "UserId");
        }
    }
}
