using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmbyStat.Core.DataStore.Migrations.Sqlite
{
    public partial class SyncTypeAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sync",
                table: "Libraries");

            migrationBuilder.AddColumn<int>(
                name: "SyncType",
                table: "Libraries",
                type: "INTEGER",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SyncType",
                table: "Libraries");

            migrationBuilder.AddColumn<bool>(
                name: "Sync",
                table: "Libraries",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
