using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace EmbyStat.Repositories.Migrations
{
    public partial class AddedStatisticsResults : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Statistics",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CalculationDateTime = table.Column<DateTime>(nullable: false),
                    JsonResult = table.Column<string>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statistics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CollectionId",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CId = table.Column<string>(nullable: true),
                    StatisticId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionId", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CollectionId_Statistics_StatisticId",
                        column: x => x.StatisticId,
                        principalTable: "Statistics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CollectionId_StatisticId",
                table: "CollectionId",
                column: "StatisticId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CollectionId");

            migrationBuilder.DropTable(
                name: "Statistics");
        }
    }
}
