using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codebreaker.Data.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class InitGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Codebreaker");

            migrationBuilder.CreateTable(
                name: "ColorGames",
                schema: "Codebreaker",
                columns: table => new
                {
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PlayerName = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: true),
                    LastMoveNumber = table.Column<int>(type: "int", nullable: false),
                    NumberCodes = table.Column<int>(type: "int", nullable: false),
                    MaxMoves = table.Column<int>(type: "int", nullable: false),
                    Won = table.Column<bool>(type: "bit", nullable: false),
                    Fields = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Codes = table.Column<string>(type: "nvarchar(140)", maxLength: 140, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColorGames", x => x.GameId);
                });

            migrationBuilder.CreateTable(
                name: "ShapeGames",
                schema: "Codebreaker",
                columns: table => new
                {
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PlayerName = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: true),
                    LastMoveNumber = table.Column<int>(type: "int", nullable: false),
                    NumberCodes = table.Column<int>(type: "int", nullable: false),
                    MaxMoves = table.Column<int>(type: "int", nullable: false),
                    Won = table.Column<bool>(type: "bit", nullable: false),
                    Fields = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Codes = table.Column<string>(type: "nvarchar(140)", maxLength: 140, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShapeGames", x => x.GameId);
                });

            migrationBuilder.CreateTable(
                name: "SimpleGames",
                schema: "Codebreaker",
                columns: table => new
                {
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PlayerName = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: true),
                    LastMoveNumber = table.Column<int>(type: "int", nullable: false),
                    NumberCodes = table.Column<int>(type: "int", nullable: false),
                    MaxMoves = table.Column<int>(type: "int", nullable: false),
                    Won = table.Column<bool>(type: "bit", nullable: false),
                    Fields = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Codes = table.Column<string>(type: "nvarchar(140)", maxLength: 140, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimpleGames", x => x.GameId);
                });

            migrationBuilder.CreateTable(
                name: "ColorMoves",
                schema: "Codebreaker",
                columns: table => new
                {
                    MoveId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MoveNumber = table.Column<int>(type: "int", nullable: false),
                    GuessPegs = table.Column<string>(type: "nvarchar(140)", maxLength: 140, nullable: false),
                    KeyPegs = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColorMoves", x => x.MoveId);
                    table.ForeignKey(
                        name: "FK_ColorMoves_ColorGames_GameId",
                        column: x => x.GameId,
                        principalSchema: "Codebreaker",
                        principalTable: "ColorGames",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShapeMoves",
                schema: "Codebreaker",
                columns: table => new
                {
                    MoveId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MoveNumber = table.Column<int>(type: "int", nullable: false),
                    GuessPegs = table.Column<string>(type: "nvarchar(140)", maxLength: 140, nullable: false),
                    KeyPegs = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShapeMoves", x => x.MoveId);
                    table.ForeignKey(
                        name: "FK_ShapeMoves_ShapeGames_GameId",
                        column: x => x.GameId,
                        principalSchema: "Codebreaker",
                        principalTable: "ShapeGames",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SimpleMoves",
                schema: "Codebreaker",
                columns: table => new
                {
                    MoveId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MoveNumber = table.Column<int>(type: "int", nullable: false),
                    GuessPegs = table.Column<string>(type: "nvarchar(140)", maxLength: 140, nullable: false),
                    KeyPegs = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimpleMoves", x => x.MoveId);
                    table.ForeignKey(
                        name: "FK_SimpleMoves_SimpleGames_GameId",
                        column: x => x.GameId,
                        principalSchema: "Codebreaker",
                        principalTable: "SimpleGames",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ColorMoves_GameId",
                schema: "Codebreaker",
                table: "ColorMoves",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_ShapeMoves_GameId",
                schema: "Codebreaker",
                table: "ShapeMoves",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_SimpleMoves_GameId",
                schema: "Codebreaker",
                table: "SimpleMoves",
                column: "GameId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ColorMoves",
                schema: "Codebreaker");

            migrationBuilder.DropTable(
                name: "ShapeMoves",
                schema: "Codebreaker");

            migrationBuilder.DropTable(
                name: "SimpleMoves",
                schema: "Codebreaker");

            migrationBuilder.DropTable(
                name: "ColorGames",
                schema: "Codebreaker");

            migrationBuilder.DropTable(
                name: "ShapeGames",
                schema: "Codebreaker");

            migrationBuilder.DropTable(
                name: "SimpleGames",
                schema: "Codebreaker");
        }
    }
}
