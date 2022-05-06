using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmbyStat.Migrations.Sqlite
{
    public partial class FixingUserViewsAgain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MediaServerUserView",
                table: "MediaServerUserView");

            migrationBuilder.DropIndex(
                name: "IX_MediaServerUserView_UserId",
                table: "MediaServerUserView");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "MediaServerUserView");

            migrationBuilder.AlterColumn<string>(
                name: "MediaId",
                table: "MediaServerUserView",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MediaServerUserView",
                table: "MediaServerUserView",
                columns: new[] { "UserId", "MediaId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MediaServerUserView",
                table: "MediaServerUserView");

            migrationBuilder.AlterColumn<string>(
                name: "MediaId",
                table: "MediaServerUserView",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "MediaServerUserView",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_MediaServerUserView",
                table: "MediaServerUserView",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_MediaServerUserView_UserId",
                table: "MediaServerUserView",
                column: "UserId");
        }
    }
}
