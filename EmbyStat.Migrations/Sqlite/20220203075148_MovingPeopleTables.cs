using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmbyStat.Migrations.Sqlite
{
    public partial class MovingPeopleTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SqlShowSqlPerson");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SqlMovieSqlPerson",
                table: "SqlMovieSqlPerson");

            migrationBuilder.AddColumn<string>(
                name: "ShowId",
                table: "SqlMovieSqlPerson",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EpisodeId",
                table: "SqlMovieSqlPerson",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SqlMovieSqlPerson",
                table: "SqlMovieSqlPerson",
                columns: new[] { "ShowId", "MovieId", "EpisodeId", "PersonId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_SqlMovieSqlPerson_EpisodeId",
                table: "SqlMovieSqlPerson",
                column: "EpisodeId");

            migrationBuilder.CreateIndex(
                name: "IX_SqlMovieSqlPerson_MovieId",
                table: "SqlMovieSqlPerson",
                column: "MovieId");

            migrationBuilder.AddForeignKey(
                name: "FK_SqlMovieSqlPerson_Episodes_EpisodeId",
                table: "SqlMovieSqlPerson",
                column: "EpisodeId",
                principalTable: "Episodes",
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SqlMovieSqlPerson_Episodes_EpisodeId",
                table: "SqlMovieSqlPerson");

            migrationBuilder.DropForeignKey(
                name: "FK_SqlMovieSqlPerson_Shows_ShowId",
                table: "SqlMovieSqlPerson");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SqlMovieSqlPerson",
                table: "SqlMovieSqlPerson");

            migrationBuilder.DropIndex(
                name: "IX_SqlMovieSqlPerson_EpisodeId",
                table: "SqlMovieSqlPerson");

            migrationBuilder.DropIndex(
                name: "IX_SqlMovieSqlPerson_MovieId",
                table: "SqlMovieSqlPerson");

            migrationBuilder.DropColumn(
                name: "ShowId",
                table: "SqlMovieSqlPerson");

            migrationBuilder.DropColumn(
                name: "EpisodeId",
                table: "SqlMovieSqlPerson");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SqlMovieSqlPerson",
                table: "SqlMovieSqlPerson",
                columns: new[] { "MovieId", "PersonId", "Type" });

            migrationBuilder.CreateTable(
                name: "SqlShowSqlPerson",
                columns: table => new
                {
                    ShowId = table.Column<string>(type: "TEXT", nullable: false),
                    PersonId = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    SqlEpisodeId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SqlShowSqlPerson", x => new { x.ShowId, x.PersonId, x.Type });
                    table.ForeignKey(
                        name: "FK_SqlShowSqlPerson_Episodes_SqlEpisodeId",
                        column: x => x.SqlEpisodeId,
                        principalTable: "Episodes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SqlShowSqlPerson_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SqlShowSqlPerson_Shows_ShowId",
                        column: x => x.ShowId,
                        principalTable: "Shows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SqlShowSqlPerson_PersonId",
                table: "SqlShowSqlPerson",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_SqlShowSqlPerson_SqlEpisodeId",
                table: "SqlShowSqlPerson",
                column: "SqlEpisodeId");
        }
    }
}
