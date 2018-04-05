using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace EmbyStat.Repositories.Migrations
{
    public partial class AddedPing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pings",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Success = table.Column<bool>(nullable: false),
                    Time = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pings", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pings");
        }
    }
}
