using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class SearchResultsMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SearchKeys",
                columns: table => new
                {
                    SearchKeyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VideoListEtag = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NextToken = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchKeys", x => x.SearchKeyId);
                });

            migrationBuilder.CreateTable(
                name: "SearchSuggestions",
                columns: table => new
                {
                    SearchSuggestionsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Suggestions = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchSuggestions", x => x.SearchSuggestionsId);
                });

            migrationBuilder.CreateTable(
                name: "VideoResults",
                columns: table => new
                {
                    SearchResultedVideoId = table.Column<int>(type: "int", nullable: false)
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
                    ChannelId = table.Column<int>(type: "int", nullable: false),
                    SearchKeyId = table.Column<int>(type: "int", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_VideoResults_ChannelId",
                table: "VideoResults",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoResults_SearchKeyId",
                table: "VideoResults",
                column: "SearchKeyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SearchSuggestions");

            migrationBuilder.DropTable(
                name: "VideoResults");

            migrationBuilder.DropTable(
                name: "SearchKeys");
        }
    }
}
