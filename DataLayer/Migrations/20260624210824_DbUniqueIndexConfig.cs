using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class DbUniqueIndexConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_History_UserId",
                table: "History");

            migrationBuilder.AlterColumn<string>(
                name: "VId",
                table: "VideosWatched",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "VId",
                table: "Videos",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "NextToken",
                table: "Videos",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VId",
                table: "VideoResults",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "NextToken",
                table: "VideoResults",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VCId",
                table: "VideoCategories",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "UId",
                table: "Users",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "SearchSuggestions",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "SearchKeys",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CId",
                table: "LiveComments",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CId",
                table: "Comments",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ChId",
                table: "Channels",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_VideosWatched_VId",
                table: "VideosWatched",
                column: "VId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Videos_VId_NextToken",
                table: "Videos",
                columns: new[] { "VId", "NextToken" },
                unique: true,
                filter: "[NextToken] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_VideoResults_VId_NextToken",
                table: "VideoResults",
                columns: new[] { "VId", "NextToken" },
                unique: true,
                filter: "[NextToken] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_VideoCategories_VCId",
                table: "VideoCategories",
                column: "VCId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UId",
                table: "Users",
                column: "UId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SearchSuggestions_Title",
                table: "SearchSuggestions",
                column: "Title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SearchKeys_Title",
                table: "SearchKeys",
                column: "Title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LiveComments_CId",
                table: "LiveComments",
                column: "CId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_History_UserId",
                table: "History",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CId",
                table: "Comments",
                column: "CId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Channels_ChId",
                table: "Channels",
                column: "ChId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VideosWatched_VId",
                table: "VideosWatched");

            migrationBuilder.DropIndex(
                name: "IX_Videos_VId_NextToken",
                table: "Videos");

            migrationBuilder.DropIndex(
                name: "IX_VideoResults_VId_NextToken",
                table: "VideoResults");

            migrationBuilder.DropIndex(
                name: "IX_VideoCategories_VCId",
                table: "VideoCategories");

            migrationBuilder.DropIndex(
                name: "IX_Users_UId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_SearchSuggestions_Title",
                table: "SearchSuggestions");

            migrationBuilder.DropIndex(
                name: "IX_SearchKeys_Title",
                table: "SearchKeys");

            migrationBuilder.DropIndex(
                name: "IX_LiveComments_CId",
                table: "LiveComments");

            migrationBuilder.DropIndex(
                name: "IX_History_UserId",
                table: "History");

            migrationBuilder.DropIndex(
                name: "IX_Comments_CId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Channels_ChId",
                table: "Channels");

            migrationBuilder.AlterColumn<string>(
                name: "VId",
                table: "VideosWatched",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "VId",
                table: "Videos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "NextToken",
                table: "Videos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VId",
                table: "VideoResults",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "NextToken",
                table: "VideoResults",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VCId",
                table: "VideoCategories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "UId",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "SearchSuggestions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "SearchKeys",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "CId",
                table: "LiveComments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "CId",
                table: "Comments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ChId",
                table: "Channels",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_History_UserId",
                table: "History",
                column: "UserId");
        }
    }
}
