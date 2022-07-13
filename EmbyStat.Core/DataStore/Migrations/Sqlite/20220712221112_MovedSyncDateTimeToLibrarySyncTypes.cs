using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmbyStat.Core.DataStore.Migrations.Sqlite
{
    public partial class MovedSyncDateTimeToLibrarySyncTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LibrarySyncType_Libraries_LibraryId",
                table: "LibrarySyncType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LibrarySyncType",
                table: "LibrarySyncType");

            migrationBuilder.DropColumn(
                name: "LastSynced",
                table: "Libraries");

            migrationBuilder.DropColumn(
                name: "SyncType",
                table: "Libraries");

            migrationBuilder.RenameTable(
                name: "LibrarySyncType",
                newName: "LibrarySyncTypes");

            migrationBuilder.RenameIndex(
                name: "IX_LibrarySyncType_LibraryId",
                table: "LibrarySyncTypes",
                newName: "IX_LibrarySyncTypes_LibraryId");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSynced",
                table: "LibrarySyncTypes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LibrarySyncTypes",
                table: "LibrarySyncTypes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LibrarySyncTypes_Libraries_LibraryId",
                table: "LibrarySyncTypes",
                column: "LibraryId",
                principalTable: "Libraries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LibrarySyncTypes_Libraries_LibraryId",
                table: "LibrarySyncTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LibrarySyncTypes",
                table: "LibrarySyncTypes");

            migrationBuilder.DropColumn(
                name: "LastSynced",
                table: "LibrarySyncTypes");

            migrationBuilder.RenameTable(
                name: "LibrarySyncTypes",
                newName: "LibrarySyncType");

            migrationBuilder.RenameIndex(
                name: "IX_LibrarySyncTypes_LibraryId",
                table: "LibrarySyncType",
                newName: "IX_LibrarySyncType_LibraryId");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSynced",
                table: "Libraries",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SyncType",
                table: "Libraries",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LibrarySyncType",
                table: "LibrarySyncType",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LibrarySyncType_Libraries_LibraryId",
                table: "LibrarySyncType",
                column: "LibraryId",
                principalTable: "Libraries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
