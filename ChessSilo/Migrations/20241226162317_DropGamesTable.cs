using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChessSilo.Migrations
{
    /// <inheritdoc />
    public partial class DropGamesTable : Migration
    {
        /// <inheritdoc />
        // protected override void Up(MigrationBuilder migrationBuilder)
        // {
        //     migrationBuilder.CreateTable(
        //         name: "Games",
        //         columns: table => new
        //         {
        //             GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
        //             Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
        //             StartedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PK_Games", x => x.GameId);
        //         });

        //     migrationBuilder.CreateIndex(
        //         name: "IX_Games_StartedOn",
        //         table: "Games",
        //         column: "StartedOn");
        // }

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
