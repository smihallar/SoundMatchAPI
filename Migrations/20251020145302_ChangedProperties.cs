using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoundMatchAPI.Migrations
{
    /// <inheritdoc />
    public partial class ChangedProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChatIds",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "FavoriteArtistIds",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "FavoriteGenreIds",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "FavoriteSongIds",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "MatchIdsAsInitiator",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "MatchIdsAsRecipient",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChatIds",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FavoriteArtistIds",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FavoriteGenreIds",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FavoriteSongIds",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MatchIdsAsInitiator",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MatchIdsAsRecipient",
                table: "AspNetUsers");
        }
    }
}
