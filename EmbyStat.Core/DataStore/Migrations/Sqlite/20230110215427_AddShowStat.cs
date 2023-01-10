using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmbyStat.Core.DataStore.Migrations.Sqlite
{
    public partial class AddShowStat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "StatisticCards",
                columns: new[] { "Id", "CardType", "Data", "Type", "UniqueType" },
                values: new object[] { new Guid("2444dff6-58cb-41ca-9fbb-a82e18f447a9"), 0, null, 1, 50 });

            migrationBuilder.InsertData(
                table: "StatisticPageCards",
                columns: new[] { "StatisticCardId", "StatisticPageId", "Order" },
                values: new object[] { new Guid("2444dff6-58cb-41ca-9fbb-a82e18f447a9"), new Guid("14928b72-f248-4442-b1ce-2c0f96eb543b"), 10 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "StatisticPageCards",
                keyColumns: new[] { "StatisticCardId", "StatisticPageId" },
                keyValues: new object[] { new Guid("2444dff6-58cb-41ca-9fbb-a82e18f447a9"), new Guid("14928b72-f248-4442-b1ce-2c0f96eb543b") });

            migrationBuilder.DeleteData(
                table: "StatisticCards",
                keyColumn: "Id",
                keyValue: new Guid("2444dff6-58cb-41ca-9fbb-a82e18f447a9"));
        }
    }
}
