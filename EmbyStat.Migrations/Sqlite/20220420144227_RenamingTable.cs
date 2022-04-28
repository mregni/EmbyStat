using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmbyStat.Migrations.Sqlite
{
    public partial class RenamingTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
                newName: "MediaServerUserViews");

            migrationBuilder.RenameIndex(
                name: "IX_MediaServerUsersViews_MovieId",
                table: "MediaServerUserViews",
                newName: "IX_MediaServerUserViews_MovieId");

            migrationBuilder.RenameIndex(
                name: "IX_MediaServerUsersViews_EpisodeId",
                table: "MediaServerUserViews",
                newName: "IX_MediaServerUserViews_EpisodeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MediaServerUserViews",
                table: "MediaServerUserViews",
                columns: new[] { "UserId", "MediaId" });

            migrationBuilder.AddForeignKey(
                name: "FK_MediaServerUserViews_Episodes_EpisodeId",
                table: "MediaServerUserViews",
                column: "EpisodeId",
                principalTable: "Episodes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaServerUserViews_MediaServerUsers_UserId",
                table: "MediaServerUserViews",
                column: "UserId",
                principalTable: "MediaServerUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MediaServerUserViews_Movies_MovieId",
                table: "MediaServerUserViews",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaServerUserViews_Episodes_EpisodeId",
                table: "MediaServerUserViews");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaServerUserViews_MediaServerUsers_UserId",
                table: "MediaServerUserViews");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaServerUserViews_Movies_MovieId",
                table: "MediaServerUserViews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MediaServerUserViews",
                table: "MediaServerUserViews");

            migrationBuilder.RenameTable(
                name: "MediaServerUserViews",
                newName: "MediaServerUsersViews");

            migrationBuilder.RenameIndex(
                name: "IX_MediaServerUserViews_MovieId",
                table: "MediaServerUsersViews",
                newName: "IX_MediaServerUsersViews_MovieId");

            migrationBuilder.RenameIndex(
                name: "IX_MediaServerUserViews_EpisodeId",
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
    }
}
