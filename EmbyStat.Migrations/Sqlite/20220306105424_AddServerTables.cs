using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmbyStat.Migrations.Sqlite
{
    public partial class AddServerTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "CommunityRating",
                table: "Shows",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "REAL",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CommunityRating",
                table: "Movies",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "REAL",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CommunityRating",
                table: "Episodes",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "REAL",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Plugins",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Version = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    ImageTag = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plugins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServerInfo",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    SystemUpdateLevel = table.Column<string>(type: "TEXT", nullable: true),
                    OperatingSystemDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    HasPendingRestart = table.Column<bool>(type: "INTEGER", nullable: false),
                    SupportsLibraryMonitor = table.Column<bool>(type: "INTEGER", nullable: false),
                    WebSocketPortNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    CanSelfRestart = table.Column<bool>(type: "INTEGER", nullable: false),
                    CanSelfUpdate = table.Column<bool>(type: "INTEGER", nullable: false),
                    CanLaunchWebBrowser = table.Column<bool>(type: "INTEGER", nullable: false),
                    ProgramDataPath = table.Column<string>(type: "TEXT", nullable: true),
                    ItemsByNamePath = table.Column<string>(type: "TEXT", nullable: true),
                    CachePath = table.Column<string>(type: "TEXT", nullable: true),
                    LogPath = table.Column<string>(type: "TEXT", nullable: true),
                    InternalMetadataPath = table.Column<string>(type: "TEXT", nullable: true),
                    TranscodingTempPath = table.Column<string>(type: "TEXT", nullable: true),
                    HttpServerPortNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    SupportsHttps = table.Column<bool>(type: "INTEGER", nullable: false),
                    HttpsPortNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    HasUpdateAvailable = table.Column<bool>(type: "INTEGER", nullable: false),
                    SupportsAutoRunAtStartup = table.Column<bool>(type: "INTEGER", nullable: false),
                    HardwareAccelerationRequiresPremiere = table.Column<bool>(type: "INTEGER", nullable: false),
                    LocalAddress = table.Column<string>(type: "TEXT", nullable: true),
                    WanAddress = table.Column<string>(type: "TEXT", nullable: true),
                    ServerName = table.Column<string>(type: "TEXT", nullable: true),
                    Version = table.Column<string>(type: "TEXT", nullable: true),
                    OperatingSystem = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerInfo", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Plugins");

            migrationBuilder.DropTable(
                name: "ServerInfo");

            migrationBuilder.AlterColumn<float>(
                name: "CommunityRating",
                table: "Shows",
                type: "REAL",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "CommunityRating",
                table: "Movies",
                type: "REAL",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "CommunityRating",
                table: "Episodes",
                type: "REAL",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
