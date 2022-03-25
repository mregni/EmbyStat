using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmbyStat.Migrations.Sqlite
{
    public partial class AddedKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Filters",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.CreateIndex(
                name: "IX_VideoStreams_AverageFrameRate",
                table: "VideoStreams",
                column: "AverageFrameRate");

            migrationBuilder.CreateIndex(
                name: "IX_VideoStreams_BitDepth",
                table: "VideoStreams",
                column: "BitDepth");

            migrationBuilder.CreateIndex(
                name: "IX_VideoStreams_Codec",
                table: "VideoStreams",
                column: "Codec");

            migrationBuilder.CreateIndex(
                name: "IX_VideoStreams_Height",
                table: "VideoStreams",
                column: "Height");

            migrationBuilder.CreateIndex(
                name: "IX_VideoStreams_VideoRange",
                table: "VideoStreams",
                column: "VideoRange");

            migrationBuilder.CreateIndex(
                name: "IX_VideoStreams_Width",
                table: "VideoStreams",
                column: "Width");

            migrationBuilder.CreateIndex(
                name: "IX_SubtitleStreams_Language",
                table: "SubtitleStreams",
                column: "Language");

            migrationBuilder.CreateIndex(
                name: "IX_Shows_CommunityRating",
                table: "Shows",
                column: "CommunityRating");

            migrationBuilder.CreateIndex(
                name: "IX_Shows_Logo",
                table: "Shows",
                column: "Logo");

            migrationBuilder.CreateIndex(
                name: "IX_Shows_Name",
                table: "Shows",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Shows_Primary",
                table: "Shows",
                column: "Primary");

            migrationBuilder.CreateIndex(
                name: "IX_Shows_RunTimeTicks",
                table: "Shows",
                column: "RunTimeTicks");

            migrationBuilder.CreateIndex(
                name: "IX_Shows_SortName",
                table: "Shows",
                column: "SortName");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_CommunityRating",
                table: "Movies",
                column: "CommunityRating");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Logo",
                table: "Movies",
                column: "Logo");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Name",
                table: "Movies",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Primary",
                table: "Movies",
                column: "Primary");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_RunTimeTicks",
                table: "Movies",
                column: "RunTimeTicks");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_SortName",
                table: "Movies",
                column: "SortName");

            migrationBuilder.CreateIndex(
                name: "IX_MediaSources_SizeInMb",
                table: "MediaSources",
                column: "SizeInMb");

            migrationBuilder.CreateIndex(
                name: "IX_Genres_Name",
                table: "Genres",
                column: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VideoStreams_AverageFrameRate",
                table: "VideoStreams");

            migrationBuilder.DropIndex(
                name: "IX_VideoStreams_BitDepth",
                table: "VideoStreams");

            migrationBuilder.DropIndex(
                name: "IX_VideoStreams_Codec",
                table: "VideoStreams");

            migrationBuilder.DropIndex(
                name: "IX_VideoStreams_Height",
                table: "VideoStreams");

            migrationBuilder.DropIndex(
                name: "IX_VideoStreams_VideoRange",
                table: "VideoStreams");

            migrationBuilder.DropIndex(
                name: "IX_VideoStreams_Width",
                table: "VideoStreams");

            migrationBuilder.DropIndex(
                name: "IX_SubtitleStreams_Language",
                table: "SubtitleStreams");

            migrationBuilder.DropIndex(
                name: "IX_Shows_CommunityRating",
                table: "Shows");

            migrationBuilder.DropIndex(
                name: "IX_Shows_Logo",
                table: "Shows");

            migrationBuilder.DropIndex(
                name: "IX_Shows_Name",
                table: "Shows");

            migrationBuilder.DropIndex(
                name: "IX_Shows_Primary",
                table: "Shows");

            migrationBuilder.DropIndex(
                name: "IX_Shows_RunTimeTicks",
                table: "Shows");

            migrationBuilder.DropIndex(
                name: "IX_Shows_SortName",
                table: "Shows");

            migrationBuilder.DropIndex(
                name: "IX_Movies_CommunityRating",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_Logo",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_Name",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_Primary",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_RunTimeTicks",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_SortName",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_MediaSources_SizeInMb",
                table: "MediaSources");

            migrationBuilder.DropIndex(
                name: "IX_Genres_Name",
                table: "Genres");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Filters",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);
        }
    }
}
