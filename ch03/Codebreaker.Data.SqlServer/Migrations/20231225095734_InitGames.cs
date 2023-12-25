using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codebreaker.Data.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class InitGames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "codebreaker");

            migrationBuilder.CreateTable(
                name: "Games",
                schema: "codebreaker",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PlayerName = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: true),
                    LastMoveNumber = table.Column<int>(type: "int", nullable: false),
                    NumberCodes = table.Column<int>(type: "int", nullable: false),
                    MaxMoves = table.Column<int>(type: "int", nullable: false),
                    IsVictory = table.Column<bool>(type: "bit", nullable: false),
                    Fields = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Codes = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Moves",
                schema: "codebreaker",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MoveNumber = table.Column<int>(type: "int", nullable: false),
                    GuessPegs = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    KeyPegs = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Moves", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Moves_Games_GameId",
                        column: x => x.GameId,
                        principalSchema: "codebreaker",
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Moves_GameId",
                schema: "codebreaker",
                table: "Moves",
                column: "GameId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Moves",
                schema: "codebreaker");

            migrationBuilder.DropTable(
                name: "Games",
                schema: "codebreaker");
        }
    }
}
