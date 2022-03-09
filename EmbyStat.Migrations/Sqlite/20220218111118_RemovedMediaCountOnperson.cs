using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmbyStat.Migrations.Sqlite
{
    public partial class RemovedMediaCountOnperson : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MovieCount",
                table: "People");

            migrationBuilder.DropColumn(
                name: "ShowCount",
                table: "People");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MovieCount",
                table: "People",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShowCount",
                table: "People",
                type: "INTEGER",
                nullable: true);
        }
    }
}
