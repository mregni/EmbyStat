using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmbyStat.Migrations.Sqlite
{
    public partial class RenamingTableAgain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SqlMediaSqlPerson_Episodes_EpisodeId",
                table: "SqlMediaSqlPerson");

            migrationBuilder.DropForeignKey(
                name: "FK_SqlMediaSqlPerson_Movies_MovieId",
                table: "SqlMediaSqlPerson");

            migrationBuilder.DropForeignKey(
                name: "FK_SqlMediaSqlPerson_People_PersonId",
                table: "SqlMediaSqlPerson");

            migrationBuilder.DropForeignKey(
                name: "FK_SqlMediaSqlPerson_Shows_ShowId",
                table: "SqlMediaSqlPerson");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SqlMediaSqlPerson",
                table: "SqlMediaSqlPerson");

            migrationBuilder.RenameTable(
                name: "SqlMediaSqlPerson",
                newName: "MediaPerson");

            migrationBuilder.RenameIndex(
                name: "IX_SqlMediaSqlPerson_ShowId",
                table: "MediaPerson",
                newName: "IX_MediaPerson_ShowId");

            migrationBuilder.RenameIndex(
                name: "IX_SqlMediaSqlPerson_PersonId",
                table: "MediaPerson",
                newName: "IX_MediaPerson_PersonId");

            migrationBuilder.RenameIndex(
                name: "IX_SqlMediaSqlPerson_MovieId",
                table: "MediaPerson",
                newName: "IX_MediaPerson_MovieId");

            migrationBuilder.RenameIndex(
                name: "IX_SqlMediaSqlPerson_EpisodeId",
                table: "MediaPerson",
                newName: "IX_MediaPerson_EpisodeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MediaPerson",
                table: "MediaPerson",
                columns: new[] { "ShowId", "MovieId", "EpisodeId", "PersonId", "Type" });

            migrationBuilder.AddForeignKey(
                name: "FK_MediaPerson_Episodes_EpisodeId",
                table: "MediaPerson",
                column: "EpisodeId",
                principalTable: "Episodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MediaPerson_Movies_MovieId",
                table: "MediaPerson",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MediaPerson_People_PersonId",
                table: "MediaPerson",
                column: "PersonId",
                principalTable: "People",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MediaPerson_Shows_ShowId",
                table: "MediaPerson",
                column: "ShowId",
                principalTable: "Shows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaPerson_Episodes_EpisodeId",
                table: "MediaPerson");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaPerson_Movies_MovieId",
                table: "MediaPerson");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaPerson_People_PersonId",
                table: "MediaPerson");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaPerson_Shows_ShowId",
                table: "MediaPerson");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MediaPerson",
                table: "MediaPerson");

            migrationBuilder.RenameTable(
                name: "MediaPerson",
                newName: "SqlMediaSqlPerson");

            migrationBuilder.RenameIndex(
                name: "IX_MediaPerson_ShowId",
                table: "SqlMediaSqlPerson",
                newName: "IX_SqlMediaSqlPerson_ShowId");

            migrationBuilder.RenameIndex(
                name: "IX_MediaPerson_PersonId",
                table: "SqlMediaSqlPerson",
                newName: "IX_SqlMediaSqlPerson_PersonId");

            migrationBuilder.RenameIndex(
                name: "IX_MediaPerson_MovieId",
                table: "SqlMediaSqlPerson",
                newName: "IX_SqlMediaSqlPerson_MovieId");

            migrationBuilder.RenameIndex(
                name: "IX_MediaPerson_EpisodeId",
                table: "SqlMediaSqlPerson",
                newName: "IX_SqlMediaSqlPerson_EpisodeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SqlMediaSqlPerson",
                table: "SqlMediaSqlPerson",
                columns: new[] { "ShowId", "MovieId", "EpisodeId", "PersonId", "Type" });

            migrationBuilder.AddForeignKey(
                name: "FK_SqlMediaSqlPerson_Episodes_EpisodeId",
                table: "SqlMediaSqlPerson",
                column: "EpisodeId",
                principalTable: "Episodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SqlMediaSqlPerson_Movies_MovieId",
                table: "SqlMediaSqlPerson",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SqlMediaSqlPerson_People_PersonId",
                table: "SqlMediaSqlPerson",
                column: "PersonId",
                principalTable: "People",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SqlMediaSqlPerson_Shows_ShowId",
                table: "SqlMediaSqlPerson",
                column: "ShowId",
                principalTable: "Shows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
