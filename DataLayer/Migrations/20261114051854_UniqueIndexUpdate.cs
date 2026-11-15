using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class UniqueIndexUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Videos_VId_PageNum",
                table: "Videos");

            migrationBuilder.DropIndex(
                name: "IX_VideoResults_VId_PageNum",
                table: "VideoResults");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_VId_PageNum_VideoCategoryId",
                table: "Videos",
                columns: new[] { "VId", "PageNum", "VideoCategoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VideoResults_VId_PageNum_SearchKeyId",
                table: "VideoResults",
                columns: new[] { "VId", "PageNum", "SearchKeyId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Videos_VId_PageNum_VideoCategoryId",
                table: "Videos");

            migrationBuilder.DropIndex(
                name: "IX_VideoResults_VId_PageNum_SearchKeyId",
                table: "VideoResults");

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
        }
    }
}
