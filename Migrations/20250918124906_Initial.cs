using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoundMatchAPI.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Artists",
                columns: table => new
                {
                    ArtistId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArtistImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artists", x => x.ArtistId);
                });

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    GenreId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.GenreId);
                });

            migrationBuilder.CreateTable(
                name: "Songs",
                columns: table => new
                {
                    SongId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AlbumImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Songs", x => x.SongId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpotifyUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountryCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProfilePictureUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "ArtistGenre",
                columns: table => new
                {
                    ArtistId = table.Column<int>(type: "int", nullable: false),
                    GenresGenreId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtistGenre", x => new { x.ArtistId, x.GenresGenreId });
                    table.ForeignKey(
                        name: "FK_ArtistGenre_Artists_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "Artists",
                        principalColumn: "ArtistId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArtistGenre_Genres_GenresGenreId",
                        column: x => x.GenresGenreId,
                        principalTable: "Genres",
                        principalColumn: "GenreId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArtistSong",
                columns: table => new
                {
                    ArtistsArtistId = table.Column<int>(type: "int", nullable: false),
                    SongId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtistSong", x => new { x.ArtistsArtistId, x.SongId });
                    table.ForeignKey(
                        name: "FK_ArtistSong_Artists_ArtistsArtistId",
                        column: x => x.ArtistsArtistId,
                        principalTable: "Artists",
                        principalColumn: "ArtistId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArtistSong_Songs_SongId",
                        column: x => x.SongId,
                        principalTable: "Songs",
                        principalColumn: "SongId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArtistUser",
                columns: table => new
                {
                    FavoriteArtistsArtistId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtistUser", x => new { x.FavoriteArtistsArtistId, x.UserId });
                    table.ForeignKey(
                        name: "FK_ArtistUser_Artists_FavoriteArtistsArtistId",
                        column: x => x.FavoriteArtistsArtistId,
                        principalTable: "Artists",
                        principalColumn: "ArtistId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArtistUser_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GenreUser",
                columns: table => new
                {
                    FavoriteGenresGenreId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenreUser", x => new { x.FavoriteGenresGenreId, x.UserId });
                    table.ForeignKey(
                        name: "FK_GenreUser_Genres_FavoriteGenresGenreId",
                        column: x => x.FavoriteGenresGenreId,
                        principalTable: "Genres",
                        principalColumn: "GenreId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenreUser_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    MatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InitiatorUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RecipientUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompatibilityScore = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.MatchId);
                    table.ForeignKey(
                        name: "FK_Matches_Users_InitiatorUserId",
                        column: x => x.InitiatorUserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_Matches_Users_RecipientUserId",
                        column: x => x.RecipientUserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "SongUser",
                columns: table => new
                {
                    FavoriteSongsSongId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SongUser", x => new { x.FavoriteSongsSongId, x.UserId });
                    table.ForeignKey(
                        name: "FK_SongUser_Songs_FavoriteSongsSongId",
                        column: x => x.FavoriteSongsSongId,
                        principalTable: "Songs",
                        principalColumn: "SongId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SongUser_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArtistMatch",
                columns: table => new
                {
                    MatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MutualArtistsArtistId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtistMatch", x => new { x.MatchId, x.MutualArtistsArtistId });
                    table.ForeignKey(
                        name: "FK_ArtistMatch_Artists_MutualArtistsArtistId",
                        column: x => x.MutualArtistsArtistId,
                        principalTable: "Artists",
                        principalColumn: "ArtistId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArtistMatch_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "MatchId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GenreMatch",
                columns: table => new
                {
                    MatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MutualGenresGenreId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenreMatch", x => new { x.MatchId, x.MutualGenresGenreId });
                    table.ForeignKey(
                        name: "FK_GenreMatch_Genres_MutualGenresGenreId",
                        column: x => x.MutualGenresGenreId,
                        principalTable: "Genres",
                        principalColumn: "GenreId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenreMatch_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "MatchId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MatchSong",
                columns: table => new
                {
                    MatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MutualSongsSongId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchSong", x => new { x.MatchId, x.MutualSongsSongId });
                    table.ForeignKey(
                        name: "FK_MatchSong_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "MatchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MatchSong_Songs_MutualSongsSongId",
                        column: x => x.MutualSongsSongId,
                        principalTable: "Songs",
                        principalColumn: "SongId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArtistGenre_GenresGenreId",
                table: "ArtistGenre",
                column: "GenresGenreId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistMatch_MutualArtistsArtistId",
                table: "ArtistMatch",
                column: "MutualArtistsArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistSong_SongId",
                table: "ArtistSong",
                column: "SongId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistUser_UserId",
                table: "ArtistUser",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GenreMatch_MutualGenresGenreId",
                table: "GenreMatch",
                column: "MutualGenresGenreId");

            migrationBuilder.CreateIndex(
                name: "IX_GenreUser_UserId",
                table: "GenreUser",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_InitiatorUserId",
                table: "Matches",
                column: "InitiatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_RecipientUserId",
                table: "Matches",
                column: "RecipientUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchSong_MutualSongsSongId",
                table: "MatchSong",
                column: "MutualSongsSongId");

            migrationBuilder.CreateIndex(
                name: "IX_SongUser_UserId",
                table: "SongUser",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArtistGenre");

            migrationBuilder.DropTable(
                name: "ArtistMatch");

            migrationBuilder.DropTable(
                name: "ArtistSong");

            migrationBuilder.DropTable(
                name: "ArtistUser");

            migrationBuilder.DropTable(
                name: "GenreMatch");

            migrationBuilder.DropTable(
                name: "GenreUser");

            migrationBuilder.DropTable(
                name: "MatchSong");

            migrationBuilder.DropTable(
                name: "SongUser");

            migrationBuilder.DropTable(
                name: "Artists");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.DropTable(
                name: "Songs");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
