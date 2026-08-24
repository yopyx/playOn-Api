using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;
using static Shared.DataTypes;

#nullable disable

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class SeedDefaultChannel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var options = new JsonSerializerOptions();

            var t = JsonSerializer.Serialize(new Dictionary<string, ThumbnailDetails>(), options);
            var s = JsonSerializer.Serialize(new Statistics(), options);
            migrationBuilder.InsertData(
                table: "Channels",
                columns: ["ChannelId", "ChId", "Title", "Thumbnails", "Statistics"],
                values: [1, "", "-unknown_Channel-", t, s]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Channels",
                keyColumn: "ChannelId",
                keyValue: 1);
        }
    }
}
