using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmbyStat.Core.DataStore.Migrations.Sqlite
{
    public partial class ChangingSampleRage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AudioSampleRate",
                table: "MediaPlays",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AudioSampleRate",
                table: "MediaPlays",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);
        }
    }
}
