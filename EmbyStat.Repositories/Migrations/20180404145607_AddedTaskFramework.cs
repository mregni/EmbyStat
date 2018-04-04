using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace EmbyStat.Repositories.Migrations
{
    public partial class AddedTaskFramework : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaskResults",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    EndTimeUtc = table.Column<DateTime>(nullable: false),
                    ErrorMessage = table.Column<string>(nullable: true),
                    Key = table.Column<string>(nullable: true),
                    LongErrorMessage = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    StartTimeUtc = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskResults", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskTriggerInfos",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DayOfWeek = table.Column<int>(nullable: true),
                    IntervalTicks = table.Column<long>(nullable: true),
                    MaxRuntimeTicks = table.Column<long>(nullable: true),
                    TimeOfDayTicks = table.Column<long>(nullable: true),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskTriggerInfos", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskResults");

            migrationBuilder.DropTable(
                name: "TaskTriggerInfos");
        }
    }
}
