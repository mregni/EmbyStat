using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmbyStat.Migrations.Sqlite
{
    public partial class RenamingMediaTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SqlMovieSqlPerson_Episodes_EpisodeId",
                table: "SqlMovieSqlPerson");

            migrationBuilder.DropForeignKey(
                name: "FK_SqlMovieSqlPerson_Movies_MovieId",
                table: "SqlMovieSqlPerson");

            migrationBuilder.DropForeignKey(
                name: "FK_SqlMovieSqlPerson_People_PersonId",
                table: "SqlMovieSqlPerson");

            migrationBuilder.DropForeignKey(
                name: "FK_SqlMovieSqlPerson_Shows_ShowId",
                table: "SqlMovieSqlPerson");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SqlMovieSqlPerson",
                table: "SqlMovieSqlPerson");

            migrationBuilder.RenameTable(
                name: "SqlMovieSqlPerson",
                newName: "SqlMediaSqlPerson");

            migrationBuilder.RenameIndex(
                name: "IX_SqlMovieSqlPerson_PersonId",
                table: "SqlMediaSqlPerson",
                newName: "IX_SqlMediaSqlPerson_PersonId");

            migrationBuilder.RenameIndex(
                name: "IX_SqlMovieSqlPerson_MovieId",
                table: "SqlMediaSqlPerson",
                newName: "IX_SqlMediaSqlPerson_MovieId");

            migrationBuilder.RenameIndex(
                name: "IX_SqlMovieSqlPerson_EpisodeId",
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

        protected override void Down(MigrationBuilder migrationBuilder)
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
                newName: "SqlMovieSqlPerson");

            migrationBuilder.RenameIndex(
                name: "IX_SqlMediaSqlPerson_PersonId",
                table: "SqlMovieSqlPerson",
                newName: "IX_SqlMovieSqlPerson_PersonId");

            migrationBuilder.RenameIndex(
                name: "IX_SqlMediaSqlPerson_MovieId",
                table: "SqlMovieSqlPerson",
                newName: "IX_SqlMovieSqlPerson_MovieId");

            migrationBuilder.RenameIndex(
                name: "IX_SqlMediaSqlPerson_EpisodeId",
                table: "SqlMovieSqlPerson",
                newName: "IX_SqlMovieSqlPerson_EpisodeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SqlMovieSqlPerson",
                table: "SqlMovieSqlPerson",
                columns: new[] { "ShowId", "MovieId", "EpisodeId", "PersonId", "Type" });

            migrationBuilder.AddForeignKey(
                name: "FK_SqlMovieSqlPerson_Episodes_EpisodeId",
                table: "SqlMovieSqlPerson",
                column: "EpisodeId",
                principalTable: "Episodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SqlMovieSqlPerson_Movies_MovieId",
                table: "SqlMovieSqlPerson",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SqlMovieSqlPerson_People_PersonId",
                table: "SqlMovieSqlPerson",
                column: "PersonId",
                principalTable: "People",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SqlMovieSqlPerson_Shows_ShowId",
                table: "SqlMovieSqlPerson",
                column: "ShowId",
                principalTable: "Shows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
