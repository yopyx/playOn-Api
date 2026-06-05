using System.Text.Json;
using DataLayer.Extensions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Options;
using static Shared.DataTypes;

#nullable disable

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class SeedDefaultChannelMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var options = new JsonSerializerOptions();

            //var jsonConverter = new ValueConverter<Dictionary<string, ThumbnailDetails>, string>(
            //        v => JsonSerializer.Serialize(v, options),
            //        v => JsonSerializer.Deserialize<Dictionary<string, ThumbnailDetails>>(v, options)!
            // );
            //var jsonConverter2 = new ValueConverter<Statistics, string>(
            //        v => JsonSerializer.Serialize(v, options),
            //        v => JsonSerializer.Deserialize<Statistics>(v, options)!
            // );
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
