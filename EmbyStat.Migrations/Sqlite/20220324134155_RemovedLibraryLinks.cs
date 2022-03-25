using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmbyStat.Migrations.Sqlite
{
    public partial class RemovedLibraryLinks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movies_Libraries_CollectionId",
                table: "Movies");

            migrationBuilder.DropForeignKey(
                name: "FK_Shows_Libraries_CollectionId",
                table: "Shows");

            migrationBuilder.DropIndex(
                name: "IX_Shows_CollectionId",
                table: "Shows");

            migrationBuilder.DropIndex(
                name: "IX_Movies_CollectionId",
                table: "Movies");

            migrationBuilder.AddColumn<string>(
                name: "LibraryId",
                table: "Shows",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LibraryId",
                table: "Movies",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shows_LibraryId",
                table: "Shows",
                column: "LibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_LibraryId",
                table: "Movies",
                column: "LibraryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Movies_Libraries_LibraryId",
                table: "Movies",
                column: "LibraryId",
                principalTable: "Libraries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Shows_Libraries_LibraryId",
                table: "Shows",
                column: "LibraryId",
                principalTable: "Libraries",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movies_Libraries_LibraryId",
                table: "Movies");

            migrationBuilder.DropForeignKey(
                name: "FK_Shows_Libraries_LibraryId",
                table: "Shows");

            migrationBuilder.DropIndex(
                name: "IX_Shows_LibraryId",
                table: "Shows");

            migrationBuilder.DropIndex(
                name: "IX_Movies_LibraryId",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "LibraryId",
                table: "Shows");

            migrationBuilder.DropColumn(
                name: "LibraryId",
                table: "Movies");

            migrationBuilder.CreateIndex(
                name: "IX_Shows_CollectionId",
                table: "Shows",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_CollectionId",
                table: "Movies",
                column: "CollectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Movies_Libraries_CollectionId",
                table: "Movies",
                column: "CollectionId",
                principalTable: "Libraries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Shows_Libraries_CollectionId",
                table: "Shows",
                column: "CollectionId",
                principalTable: "Libraries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
