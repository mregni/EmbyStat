using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmbyStat.Core.DataStore.Migrations.Sqlite
{
    public partial class MovedSyncTypeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LibrarySyncType",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    LibraryId = table.Column<string>(type: "TEXT", nullable: false),
                    SyncType = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LibrarySyncType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LibrarySyncType_Libraries_LibraryId",
                        column: x => x.LibraryId,
                        principalTable: "Libraries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LibrarySyncType_LibraryId",
                table: "LibrarySyncType",
                column: "LibraryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LibrarySyncType");
        }
    }
}
