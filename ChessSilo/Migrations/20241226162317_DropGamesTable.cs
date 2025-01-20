using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChessSilo.Migrations
{
    /// <inheritdoc />
    public partial class DropGamesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TABLE Games;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Games");
        }
    }
}
