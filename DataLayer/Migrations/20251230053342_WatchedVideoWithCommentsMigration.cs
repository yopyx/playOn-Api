using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class WatchedVideoWithCommentsMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VideosWatched",
                columns: table => new
                {
                    VideoWatchedId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublishedAt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Thumbnails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LiveBroadcastContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Duration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Statistics = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LiveStreamingDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ListEtag = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NextToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChannelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideosWatched", x => x.VideoWatchedId);
                    table.ForeignKey(
                        name: "FK_VideosWatched_Channels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "Channels",
                        principalColumn: "ChannelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    CommentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TextOriginal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthorDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthorProfileImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LikeCount = table.Column<int>(type: "int", nullable: false),
                    PublishedAt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ListEtag = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NextToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VideoWatchedId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.CommentId);
                    table.ForeignKey(
                        name: "FK_Comment_VideosWatched_VideoWatchedId",
                        column: x => x.VideoWatchedId,
                        principalTable: "VideosWatched",
                        principalColumn: "VideoWatchedId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LiveChatComment",
                columns: table => new
                {
                    LiveChatCommentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublishedAt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfileImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ListEtag = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NextToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VideoWatchedId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiveChatComment", x => x.LiveChatCommentId);
                    table.ForeignKey(
                        name: "FK_LiveChatComment_VideosWatched_VideoWatchedId",
                        column: x => x.VideoWatchedId,
                        principalTable: "VideosWatched",
                        principalColumn: "VideoWatchedId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comment_VideoWatchedId",
                table: "Comment",
                column: "VideoWatchedId");

            migrationBuilder.CreateIndex(
                name: "IX_LiveChatComment_VideoWatchedId",
                table: "LiveChatComment",
                column: "VideoWatchedId");

            migrationBuilder.CreateIndex(
                name: "IX_VideosWatched_ChannelId",
                table: "VideosWatched",
                column: "ChannelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropTable(
                name: "LiveChatComment");

            migrationBuilder.DropTable(
                name: "VideosWatched");
        }
    }
}
