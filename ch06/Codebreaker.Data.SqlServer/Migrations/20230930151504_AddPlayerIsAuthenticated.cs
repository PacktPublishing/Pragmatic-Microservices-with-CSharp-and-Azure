using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codebreaker.Data.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerIsAuthenticated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PlayerIsAuthenticated",
                schema: "codebreaker",
                table: "Games",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlayerIsAuthenticated",
                schema: "codebreaker",
                table: "Games");
        }
    }
}
