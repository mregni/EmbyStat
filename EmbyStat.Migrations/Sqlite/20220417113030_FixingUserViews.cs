using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmbyStat.Migrations.Sqlite
{
    public partial class FixingUserViews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaServerUserView_Episodes_EpisodeId",
                table: "MediaServerUserView");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaServerUserView_Movies_MovieId",
                table: "MediaServerUserView");

            migrationBuilder.AddColumn<string>(
                name: "MediaId",
                table: "MediaServerUserView",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MediaType",
                table: "MediaServerUserView",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MediaServerUserView_Episodes_EpisodeId",
                table: "MediaServerUserView",
                column: "EpisodeId",
                principalTable: "Episodes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaServerUserView_Movies_MovieId",
                table: "MediaServerUserView",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaServerUserView_Episodes_EpisodeId",
                table: "MediaServerUserView");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaServerUserView_Movies_MovieId",
                table: "MediaServerUserView");

            migrationBuilder.DropColumn(
                name: "MediaId",
                table: "MediaServerUserView");

            migrationBuilder.DropColumn(
                name: "MediaType",
                table: "MediaServerUserView");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaServerUserView_Episodes_EpisodeId",
                table: "MediaServerUserView",
                column: "EpisodeId",
                principalTable: "Episodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_MediaServerUserView_Movies_MovieId",
                table: "MediaServerUserView",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
