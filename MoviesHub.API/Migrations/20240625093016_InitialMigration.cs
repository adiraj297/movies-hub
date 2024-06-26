using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoviesHub.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cinema",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "varchar(64)", nullable: false),
                    location = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cinema", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Movie",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    title = table.Column<string>(type: "varchar(128)", nullable: false),
                    releaseDate = table.Column<DateOnly>(type: "date", nullable: false),
                    genre = table.Column<string>(type: "varchar(64)", nullable: false),
                    runtime = table.Column<int>(type: "INTEGER", nullable: false),
                    synopsis = table.Column<string>(type: "TEXT", nullable: false),
                    director = table.Column<string>(type: "varchar(64)", nullable: false),
                    rating = table.Column<string>(type: "varchar(8)", nullable: false),
                    princessTheatreMovieId = table.Column<string>(type: "varchar(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movie", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "MovieCinema",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    movieId = table.Column<int>(type: "INTEGER", nullable: false),
                    cinemaId = table.Column<int>(type: "INTEGER", nullable: false),
                    showTime = table.Column<DateOnly>(type: "date", nullable: false),
                    ticketPrice = table.Column<decimal>(type: "decimal(4, 2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieCinema", x => x.id);
                    table.ForeignKey(
                        name: "FK_MovieCinema_Cinema_cinemaId",
                        column: x => x.cinemaId,
                        principalTable: "Cinema",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieCinema_Movie_movieId",
                        column: x => x.movieId,
                        principalTable: "Movie",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MovieReview",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    movieId = table.Column<int>(type: "INTEGER", nullable: false),
                    score = table.Column<decimal>(type: "decimal(4, 2)", nullable: false),
                    comment = table.Column<string>(type: "TEXT", nullable: true),
                    reviewDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieReview", x => x.id);
                    table.ForeignKey(
                        name: "FK_MovieReview_Movie_movieId",
                        column: x => x.movieId,
                        principalTable: "Movie",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovieCinema_cinemaId",
                table: "MovieCinema",
                column: "cinemaId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieCinema_movieId",
                table: "MovieCinema",
                column: "movieId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieReview_movieId",
                table: "MovieReview",
                column: "movieId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovieCinema");

            migrationBuilder.DropTable(
                name: "MovieReview");

            migrationBuilder.DropTable(
                name: "Cinema");

            migrationBuilder.DropTable(
                name: "Movie");
        }
    }
}
