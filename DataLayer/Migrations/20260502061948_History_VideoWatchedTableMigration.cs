using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class History_VideoWatchedTableMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VideosWatched_History_HistoryId",
                table: "VideosWatched");

            migrationBuilder.DropIndex(
                name: "IX_VideosWatched_HistoryId",
                table: "VideosWatched");

            migrationBuilder.DropColumn(
                name: "HistoryId",
                table: "VideosWatched");

            migrationBuilder.CreateTable(
                name: "History_VideoWatched",
                columns: table => new
                {
                    History_VideoWatchedId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HistoryId = table.Column<int>(type: "int", nullable: false),
                    VideoWatchedId = table.Column<int>(type: "int", nullable: false),
                    WatchedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_History_VideoWatched", x => x.History_VideoWatchedId);
                    table.ForeignKey(
                        name: "FK_History_VideoWatched_History_HistoryId",
                        column: x => x.HistoryId,
                        principalTable: "History",
                        principalColumn: "HistoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_History_VideoWatched_VideosWatched_VideoWatchedId",
                        column: x => x.VideoWatchedId,
                        principalTable: "VideosWatched",
                        principalColumn: "VideoWatchedId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_History_VideoWatched_HistoryId",
                table: "History_VideoWatched",
                column: "HistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_History_VideoWatched_VideoWatchedId",
                table: "History_VideoWatched",
                column: "VideoWatchedId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "History_VideoWatched");

            migrationBuilder.AddColumn<int>(
                name: "HistoryId",
                table: "VideosWatched",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VideosWatched_HistoryId",
                table: "VideosWatched",
                column: "HistoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_VideosWatched_History_HistoryId",
                table: "VideosWatched",
                column: "HistoryId",
                principalTable: "History",
                principalColumn: "HistoryId");
        }
    }
}
