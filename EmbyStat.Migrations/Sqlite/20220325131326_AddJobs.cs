using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmbyStat.Migrations.Sqlite
{
    public partial class AddJobs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentProgressPercentage = table.Column<double>(type: "REAL", nullable: true),
                    StartTimeUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EndTimeUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Trigger = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Jobs",
                columns: new[] { "Id", "CurrentProgressPercentage", "Description", "EndTimeUtc", "StartTimeUtc", "State", "Title", "Trigger" },
                values: new object[] { new Guid("41e0bf22-1e6b-4f5d-90be-ec966f746a2f"), 0.0, "SYSTEM-SYNCDESCRIPTION", null, null, 0, "SYSTEM-SYNC", "0 2 * * *" });

            migrationBuilder.InsertData(
                table: "Jobs",
                columns: new[] { "Id", "CurrentProgressPercentage", "Description", "EndTimeUtc", "StartTimeUtc", "State", "Title", "Trigger" },
                values: new object[] { new Guid("78bc2bf0-abd9-48ef-aeff-9c396d644f2a"), 0.0, "UPDATE-CHECKERDESCRIPTION", null, null, 0, "UPDATE-CHECKER", "0 */12 * * *" });

            migrationBuilder.InsertData(
                table: "Jobs",
                columns: new[] { "Id", "CurrentProgressPercentage", "Description", "EndTimeUtc", "StartTimeUtc", "State", "Title", "Trigger" },
                values: new object[] { new Guid("b109ca73-0563-4062-a3e2-f7e6a00b73e9"), 0.0, "DATABASE-CLEANUPDESCRIPTION", null, null, 0, "DATABASE-CLEANUP", "0 4 * * *" });

            migrationBuilder.InsertData(
                table: "Jobs",
                columns: new[] { "Id", "CurrentProgressPercentage", "Description", "EndTimeUtc", "StartTimeUtc", "State", "Title", "Trigger" },
                values: new object[] { new Guid("be68900b-ee1d-41ef-b12f-60ef3106052e"), 0.0, "SHOW-SYNCDESCRIPTION", null, null, 0, "SHOW-SYNC", "0 3 * * *" });

            migrationBuilder.InsertData(
                table: "Jobs",
                columns: new[] { "Id", "CurrentProgressPercentage", "Description", "EndTimeUtc", "StartTimeUtc", "State", "Title", "Trigger" },
                values: new object[] { new Guid("c40555dc-ea57-4c6e-a225-905223d31c3c"), 0.0, "MOVIE-SYNCDESCRIPTION", null, null, 0, "MOVIE-SYNC", "0 32 * * *" });

            migrationBuilder.InsertData(
                table: "Jobs",
                columns: new[] { "Id", "CurrentProgressPercentage", "Description", "EndTimeUtc", "StartTimeUtc", "State", "Title", "Trigger" },
                values: new object[] { new Guid("ce1fbc9e-21ee-450b-9cdf-58a0e17ea98e"), 0.0, "PINGDESCRIPTION", null, null, 0, "PING", "*/5 * * * *" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Jobs");
        }
    }
}
