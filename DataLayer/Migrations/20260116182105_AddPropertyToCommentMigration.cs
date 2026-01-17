using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyToCommentMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LiveChatComment_VideosWatched_VideoWatchedId",
                table: "LiveChatComment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LiveChatComment",
                table: "LiveChatComment");

            migrationBuilder.RenameTable(
                name: "LiveChatComment",
                newName: "LiveComments");

            migrationBuilder.RenameIndex(
                name: "IX_LiveChatComment_VideoWatchedId",
                table: "LiveComments",
                newName: "IX_LiveComments_VideoWatchedId");

            migrationBuilder.AddColumn<int>(
                name: "TotalReplyCount",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LiveComments",
                table: "LiveComments",
                column: "LiveChatCommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_LiveComments_VideosWatched_VideoWatchedId",
                table: "LiveComments",
                column: "VideoWatchedId",
                principalTable: "VideosWatched",
                principalColumn: "VideoWatchedId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LiveComments_VideosWatched_VideoWatchedId",
                table: "LiveComments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LiveComments",
                table: "LiveComments");

            migrationBuilder.DropColumn(
                name: "TotalReplyCount",
                table: "Comments");

            migrationBuilder.RenameTable(
                name: "LiveComments",
                newName: "LiveChatComment");

            migrationBuilder.RenameIndex(
                name: "IX_LiveComments_VideoWatchedId",
                table: "LiveChatComment",
                newName: "IX_LiveChatComment_VideoWatchedId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LiveChatComment",
                table: "LiveChatComment",
                column: "LiveChatCommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_LiveChatComment_VideosWatched_VideoWatchedId",
                table: "LiveChatComment",
                column: "VideoWatchedId",
                principalTable: "VideosWatched",
                principalColumn: "VideoWatchedId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
