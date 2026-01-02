using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class RepliesMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_VideosWatched_VideoWatchedId",
                table: "Comment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comment",
                table: "Comment");

            migrationBuilder.RenameTable(
                name: "Comment",
                newName: "Comments");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_VideoWatchedId",
                table: "Comments",
                newName: "IX_Comments_VideoWatchedId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comments",
                table: "Comments",
                column: "CommentId");

            migrationBuilder.CreateTable(
                name: "NestedComment",
                columns: table => new
                {
                    NestedCommentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParentId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TextOriginal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthorDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthorProfileImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LikeCount = table.Column<int>(type: "int", nullable: false),
                    PublishedAt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NestedComment", x => x.NestedCommentId);
                    table.ForeignKey(
                        name: "FK_NestedComment_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "CommentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NestedComment_CommentId",
                table: "NestedComment",
                column: "CommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_VideosWatched_VideoWatchedId",
                table: "Comments",
                column: "VideoWatchedId",
                principalTable: "VideosWatched",
                principalColumn: "VideoWatchedId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_VideosWatched_VideoWatchedId",
                table: "Comments");

            migrationBuilder.DropTable(
                name: "NestedComment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comments",
                table: "Comments");

            migrationBuilder.RenameTable(
                name: "Comments",
                newName: "Comment");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_VideoWatchedId",
                table: "Comment",
                newName: "IX_Comment_VideoWatchedId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comment",
                table: "Comment",
                column: "CommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_VideosWatched_VideoWatchedId",
                table: "Comment",
                column: "VideoWatchedId",
                principalTable: "VideosWatched",
                principalColumn: "VideoWatchedId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
