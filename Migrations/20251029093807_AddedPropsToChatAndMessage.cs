using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoundMatchAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedPropsToChatAndMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_SenderId",
                table: "Messages");

            migrationBuilder.AddColumn<string>(
                name: "ChatId",
                table: "Matches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MatchId",
                table: "Chats",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_MatchId",
                table: "Chats",
                column: "MatchId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Matches_MatchId",
                table: "Chats",
                column: "MatchId",
                principalTable: "Matches",
                principalColumn: "MatchId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_SenderId",
                table: "Messages",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Matches_MatchId",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_SenderId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Chats_MatchId",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "ChatId",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "MatchId",
                table: "Chats");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_SenderId",
                table: "Messages",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
