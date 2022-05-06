using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmbyStat.Migrations.Sqlite
{
    public partial class RenaingTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaServerUserView_Episodes_EpisodeId",
                table: "MediaServerUserView");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaServerUserView_MediaServerUsers_UserId",
                table: "MediaServerUserView");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaServerUserView_Movies_MovieId",
                table: "MediaServerUserView");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MediaServerUserView",
                table: "MediaServerUserView");

            migrationBuilder.RenameTable(
                name: "MediaServerUserView",
                newName: "MediaServerUsersViews");

            migrationBuilder.RenameIndex(
                name: "IX_MediaServerUserView_MovieId",
                table: "MediaServerUsersViews",
                newName: "IX_MediaServerUsersViews_MovieId");

            migrationBuilder.RenameIndex(
                name: "IX_MediaServerUserView_EpisodeId",
                table: "MediaServerUsersViews",
                newName: "IX_MediaServerUsersViews_EpisodeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MediaServerUsersViews",
                table: "MediaServerUsersViews",
                columns: new[] { "UserId", "MediaId" });

            migrationBuilder.AddForeignKey(
                name: "FK_MediaServerUsersViews_Episodes_EpisodeId",
                table: "MediaServerUsersViews",
                column: "EpisodeId",
                principalTable: "Episodes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaServerUsersViews_MediaServerUsers_UserId",
                table: "MediaServerUsersViews",
                column: "UserId",
                principalTable: "MediaServerUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MediaServerUsersViews_Movies_MovieId",
                table: "MediaServerUsersViews",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaServerUsersViews_Episodes_EpisodeId",
                table: "MediaServerUsersViews");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaServerUsersViews_MediaServerUsers_UserId",
                table: "MediaServerUsersViews");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaServerUsersViews_Movies_MovieId",
                table: "MediaServerUsersViews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MediaServerUsersViews",
                table: "MediaServerUsersViews");

            migrationBuilder.RenameTable(
                name: "MediaServerUsersViews",
                newName: "MediaServerUserView");

            migrationBuilder.RenameIndex(
                name: "IX_MediaServerUsersViews_MovieId",
                table: "MediaServerUserView",
                newName: "IX_MediaServerUserView_MovieId");

            migrationBuilder.RenameIndex(
                name: "IX_MediaServerUsersViews_EpisodeId",
                table: "MediaServerUserView",
                newName: "IX_MediaServerUserView_EpisodeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MediaServerUserView",
                table: "MediaServerUserView",
                columns: new[] { "UserId", "MediaId" });

            migrationBuilder.AddForeignKey(
                name: "FK_MediaServerUserView_Episodes_EpisodeId",
                table: "MediaServerUserView",
                column: "EpisodeId",
                principalTable: "Episodes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaServerUserView_MediaServerUsers_UserId",
                table: "MediaServerUserView",
                column: "UserId",
                principalTable: "MediaServerUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MediaServerUserView_Movies_MovieId",
                table: "MediaServerUserView",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id");
        }
    }
}
