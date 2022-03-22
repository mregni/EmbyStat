using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmbyStat.Migrations.Sqlite
{
    public partial class AddDeleteCascade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserConfigurations_Users_UserId",
                table: "UserConfigurations");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPolicies_Users_UserId",
                table: "UserPolicies");

            migrationBuilder.AddForeignKey(
                name: "FK_UserConfigurations_Users_UserId",
                table: "UserConfigurations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPolicies_Users_UserId",
                table: "UserPolicies",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserConfigurations_Users_UserId",
                table: "UserConfigurations");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPolicies_Users_UserId",
                table: "UserPolicies");

            migrationBuilder.AddForeignKey(
                name: "FK_UserConfigurations_Users_UserId",
                table: "UserConfigurations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPolicies_Users_UserId",
                table: "UserPolicies",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
