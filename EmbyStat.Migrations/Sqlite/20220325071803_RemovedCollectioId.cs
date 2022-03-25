using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmbyStat.Migrations.Sqlite
{
    public partial class RemovedCollectioId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CollectionId",
                table: "Shows");

            migrationBuilder.DropColumn(
                name: "CollectionId",
                table: "Seasons");

            migrationBuilder.DropColumn(
                name: "CollectionId",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "CollectionId",
                table: "Episodes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CollectionId",
                table: "Shows",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CollectionId",
                table: "Seasons",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CollectionId",
                table: "Movies",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CollectionId",
                table: "Episodes",
                type: "TEXT",
                nullable: true);
        }
    }
}
