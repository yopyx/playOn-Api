using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddingPropsToEntitiesMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_History_Users_UserId",
                table: "History");

            migrationBuilder.DropForeignKey(
                name: "FK_History_VideoWatched_History_HistoryId",
                table: "History_VideoWatched");

            migrationBuilder.DropForeignKey(
                name: "FK_History_VideoWatched_VideosWatched_VideoWatchedId",
                table: "History_VideoWatched");

            migrationBuilder.DropPrimaryKey(
                name: "PK_History_VideoWatched",
                table: "History_VideoWatched");

            migrationBuilder.RenameTable(
                name: "History_VideoWatched",
                newName: "VideosHistory");

            migrationBuilder.RenameIndex(
                name: "IX_History_VideoWatched_VideoWatchedId",
                table: "VideosHistory",
                newName: "IX_VideosHistory_VideoWatchedId");

            migrationBuilder.RenameIndex(
                name: "IX_History_VideoWatched_HistoryId",
                table: "VideosHistory",
                newName: "IX_VideosHistory_HistoryId");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "History",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_VideosHistory",
                table: "VideosHistory",
                column: "History_VideoWatchedId");

            migrationBuilder.AddForeignKey(
                name: "FK_History_Users_UserId",
                table: "History",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VideosHistory_History_HistoryId",
                table: "VideosHistory",
                column: "HistoryId",
                principalTable: "History",
                principalColumn: "HistoryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VideosHistory_VideosWatched_VideoWatchedId",
                table: "VideosHistory",
                column: "VideoWatchedId",
                principalTable: "VideosWatched",
                principalColumn: "VideoWatchedId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_History_Users_UserId",
                table: "History");

            migrationBuilder.DropForeignKey(
                name: "FK_VideosHistory_History_HistoryId",
                table: "VideosHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_VideosHistory_VideosWatched_VideoWatchedId",
                table: "VideosHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VideosHistory",
                table: "VideosHistory");

            migrationBuilder.RenameTable(
                name: "VideosHistory",
                newName: "History_VideoWatched");

            migrationBuilder.RenameIndex(
                name: "IX_VideosHistory_VideoWatchedId",
                table: "History_VideoWatched",
                newName: "IX_History_VideoWatched_VideoWatchedId");

            migrationBuilder.RenameIndex(
                name: "IX_VideosHistory_HistoryId",
                table: "History_VideoWatched",
                newName: "IX_History_VideoWatched_HistoryId");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "History",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_History_VideoWatched",
                table: "History_VideoWatched",
                column: "History_VideoWatchedId");

            migrationBuilder.AddForeignKey(
                name: "FK_History_Users_UserId",
                table: "History",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_History_VideoWatched_History_HistoryId",
                table: "History_VideoWatched",
                column: "HistoryId",
                principalTable: "History",
                principalColumn: "HistoryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_History_VideoWatched_VideosWatched_VideoWatchedId",
                table: "History_VideoWatched",
                column: "VideoWatchedId",
                principalTable: "VideosWatched",
                principalColumn: "VideoWatchedId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
