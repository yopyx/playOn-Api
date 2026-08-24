using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Channels",
                columns: table => new
                {
                    ChannelId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ChId = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Thumbnails = table.Column<string>(type: "text", nullable: false),
                    Statistics = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Channels", x => x.ChannelId);
                });

            migrationBuilder.CreateTable(
                name: "DataProtectionKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FriendlyName = table.Column<string>(type: "text", nullable: true),
                    Xml = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataProtectionKeys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SearchKeys",
                columns: table => new
                {
                    SearchKeyId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    VideoListEtag = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchKeys", x => x.SearchKeyId);
                });

            migrationBuilder.CreateTable(
                name: "SearchSuggestions",
                columns: table => new
                {
                    SearchSuggestionsId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Suggestions = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchSuggestions", x => x.SearchSuggestionsId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "VideoCategories",
                columns: table => new
                {
                    VideoCategoryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VCId = table.Column<string>(type: "text", nullable: false),
                    Etag = table.Column<string>(type: "text", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoCategories", x => x.VideoCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "VideosWatched",
                columns: table => new
                {
                    VideoWatchedId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VId = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    PublishedAt = table.Column<string>(type: "text", nullable: false),
                    Thumbnails = table.Column<string>(type: "text", nullable: false),
                    Tags = table.Column<string>(type: "text", nullable: false),
                    LiveBroadcastContent = table.Column<string>(type: "text", nullable: false),
                    Duration = table.Column<string>(type: "text", nullable: false),
                    Statistics = table.Column<string>(type: "text", nullable: false),
                    LiveStreamingDetails = table.Column<string>(type: "text", nullable: false),
                    Etag = table.Column<string>(type: "text", nullable: false),
                    ChannelId = table.Column<int>(type: "integer", nullable: false)
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
                name: "VideoResults",
                columns: table => new
                {
                    SearchResultedVideoId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VId = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    PublishedAt = table.Column<string>(type: "text", nullable: false),
                    Thumbnails = table.Column<string>(type: "text", nullable: false),
                    Tags = table.Column<string>(type: "text", nullable: false),
                    LiveBroadcastContent = table.Column<string>(type: "text", nullable: false),
                    Duration = table.Column<string>(type: "text", nullable: false),
                    Statistics = table.Column<string>(type: "text", nullable: false),
                    LiveStreamingDetails = table.Column<string>(type: "text", nullable: false),
                    Etag = table.Column<string>(type: "text", nullable: false),
                    ListEtag = table.Column<string>(type: "text", nullable: false),
                    NextToken = table.Column<string>(type: "text", nullable: true),
                    ChannelId = table.Column<int>(type: "integer", nullable: false),
                    SearchKeyId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoResults", x => x.SearchResultedVideoId);
                    table.ForeignKey(
                        name: "FK_VideoResults_Channels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "Channels",
                        principalColumn: "ChannelId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VideoResults_SearchKeys_SearchKeyId",
                        column: x => x.SearchKeyId,
                        principalTable: "SearchKeys",
                        principalColumn: "SearchKeyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "History",
                columns: table => new
                {
                    HistoryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_History", x => x.HistoryId);
                    table.ForeignKey(
                        name: "FK_History_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Videos",
                columns: table => new
                {
                    VideoId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VId = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    PublishedAt = table.Column<string>(type: "text", nullable: false),
                    Thumbnails = table.Column<string>(type: "text", nullable: false),
                    Tags = table.Column<string>(type: "text", nullable: false),
                    LiveBroadcastContent = table.Column<string>(type: "text", nullable: false),
                    Duration = table.Column<string>(type: "text", nullable: false),
                    Statistics = table.Column<string>(type: "text", nullable: false),
                    LiveStreamingDetails = table.Column<string>(type: "text", nullable: false),
                    Etag = table.Column<string>(type: "text", nullable: false),
                    ListEtag = table.Column<string>(type: "text", nullable: false),
                    NextToken = table.Column<string>(type: "text", nullable: true),
                    ChannelId = table.Column<int>(type: "integer", nullable: false),
                    VideoCategoryId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Videos", x => x.VideoId);
                    table.ForeignKey(
                        name: "FK_Videos_Channels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "Channels",
                        principalColumn: "ChannelId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Videos_VideoCategories_VideoCategoryId",
                        column: x => x.VideoCategoryId,
                        principalTable: "VideoCategories",
                        principalColumn: "VideoCategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    CommentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CId = table.Column<string>(type: "text", nullable: false),
                    TextOriginal = table.Column<string>(type: "text", nullable: false),
                    AuthorDisplayName = table.Column<string>(type: "text", nullable: false),
                    AuthorProfileImageUrl = table.Column<string>(type: "text", nullable: false),
                    LikeCount = table.Column<int>(type: "integer", nullable: false),
                    TotalReplyCount = table.Column<int>(type: "integer", nullable: false),
                    PublishedAt = table.Column<string>(type: "text", nullable: false),
                    ListEtag = table.Column<string>(type: "text", nullable: false),
                    NextToken = table.Column<string>(type: "text", nullable: true),
                    VideoWatchedId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.CommentId);
                    table.ForeignKey(
                        name: "FK_Comments_VideosWatched_VideoWatchedId",
                        column: x => x.VideoWatchedId,
                        principalTable: "VideosWatched",
                        principalColumn: "VideoWatchedId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LiveComments",
                columns: table => new
                {
                    LiveChatCommentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CId = table.Column<string>(type: "text", nullable: false),
                    PublishedAt = table.Column<string>(type: "text", nullable: false),
                    DisplayMessage = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    ProfileImageUrl = table.Column<string>(type: "text", nullable: false),
                    ListEtag = table.Column<string>(type: "text", nullable: false),
                    PageNo = table.Column<string>(type: "text", nullable: false),
                    NextToken = table.Column<string>(type: "text", nullable: true),
                    PollingIntervalMillis = table.Column<int>(type: "integer", nullable: true),
                    VideoWatchedId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiveComments", x => x.LiveChatCommentId);
                    table.ForeignKey(
                        name: "FK_LiveComments_VideosWatched_VideoWatchedId",
                        column: x => x.VideoWatchedId,
                        principalTable: "VideosWatched",
                        principalColumn: "VideoWatchedId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VideosHistory",
                columns: table => new
                {
                    History_VideoWatchedId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HistoryId = table.Column<int>(type: "integer", nullable: false),
                    VideoWatchedId = table.Column<int>(type: "integer", nullable: false),
                    WatchedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideosHistory", x => x.History_VideoWatchedId);
                    table.ForeignKey(
                        name: "FK_VideosHistory_History_HistoryId",
                        column: x => x.HistoryId,
                        principalTable: "History",
                        principalColumn: "HistoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VideosHistory_VideosWatched_VideoWatchedId",
                        column: x => x.VideoWatchedId,
                        principalTable: "VideosWatched",
                        principalColumn: "VideoWatchedId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NestedComment",
                columns: table => new
                {
                    NestedCommentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CId = table.Column<string>(type: "text", nullable: false),
                    ParentId = table.Column<string>(type: "text", nullable: false),
                    TextOriginal = table.Column<string>(type: "text", nullable: false),
                    AuthorDisplayName = table.Column<string>(type: "text", nullable: false),
                    AuthorProfileImageUrl = table.Column<string>(type: "text", nullable: false),
                    LikeCount = table.Column<int>(type: "integer", nullable: false),
                    PublishedAt = table.Column<string>(type: "text", nullable: false),
                    CommentId = table.Column<int>(type: "integer", nullable: false)
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
                name: "IX_Channels_ChId",
                table: "Channels",
                column: "ChId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CId",
                table: "Comments",
                column: "CId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_VideoWatchedId",
                table: "Comments",
                column: "VideoWatchedId");

            migrationBuilder.CreateIndex(
                name: "IX_History_UserId",
                table: "History",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LiveComments_CId",
                table: "LiveComments",
                column: "CId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LiveComments_VideoWatchedId",
                table: "LiveComments",
                column: "VideoWatchedId");

            migrationBuilder.CreateIndex(
                name: "IX_NestedComment_CommentId",
                table: "NestedComment",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_SearchKeys_Title",
                table: "SearchKeys",
                column: "Title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SearchSuggestions_Title",
                table: "SearchSuggestions",
                column: "Title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UId",
                table: "Users",
                column: "UId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VideoCategories_VCId",
                table: "VideoCategories",
                column: "VCId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VideoResults_ChannelId",
                table: "VideoResults",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoResults_SearchKeyId",
                table: "VideoResults",
                column: "SearchKeyId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoResults_VId_NextToken",
                table: "VideoResults",
                columns: new[] { "VId", "NextToken" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Videos_ChannelId",
                table: "Videos",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_VId_NextToken",
                table: "Videos",
                columns: new[] { "VId", "NextToken" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Videos_VideoCategoryId",
                table: "Videos",
                column: "VideoCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_VideosHistory_HistoryId",
                table: "VideosHistory",
                column: "HistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_VideosHistory_VideoWatchedId",
                table: "VideosHistory",
                column: "VideoWatchedId");

            migrationBuilder.CreateIndex(
                name: "IX_VideosWatched_ChannelId",
                table: "VideosWatched",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_VideosWatched_VId",
                table: "VideosWatched",
                column: "VId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataProtectionKeys");

            migrationBuilder.DropTable(
                name: "LiveComments");

            migrationBuilder.DropTable(
                name: "NestedComment");

            migrationBuilder.DropTable(
                name: "SearchSuggestions");

            migrationBuilder.DropTable(
                name: "VideoResults");

            migrationBuilder.DropTable(
                name: "Videos");

            migrationBuilder.DropTable(
                name: "VideosHistory");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "SearchKeys");

            migrationBuilder.DropTable(
                name: "VideoCategories");

            migrationBuilder.DropTable(
                name: "History");

            migrationBuilder.DropTable(
                name: "VideosWatched");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Channels");
        }
    }
}
