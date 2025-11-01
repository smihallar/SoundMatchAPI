using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoundMatchAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SpotifyId",
                table: "Songs",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "SpotifyId",
                table: "Artists",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Songs_SpotifyId",
                table: "Songs",
                column: "SpotifyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Artists_SpotifyId",
                table: "Artists",
                column: "SpotifyId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Songs_SpotifyId",
                table: "Songs");

            migrationBuilder.DropIndex(
                name: "IX_Artists_SpotifyId",
                table: "Artists");

            migrationBuilder.AlterColumn<string>(
                name: "SpotifyId",
                table: "Songs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "SpotifyId",
                table: "Artists",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
