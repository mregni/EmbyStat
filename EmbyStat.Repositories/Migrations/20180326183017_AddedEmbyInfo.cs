using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace EmbyStat.Repositories.Migrations
{
    public partial class AddedEmbyInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Plugins",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AssemblyFileName = table.Column<string>(nullable: true),
                    ConfigurationDateLastModified = table.Column<DateTime>(nullable: false),
                    ConfigurationFileName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ImageUrl = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Version = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plugins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServerInfo",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CachePath = table.Column<string>(nullable: true),
                    CanLaunchWebBrowser = table.Column<bool>(nullable: false),
                    CanSelfRestart = table.Column<bool>(nullable: false),
                    CanSelfUpdate = table.Column<bool>(nullable: false),
                    EncoderLocationType = table.Column<string>(nullable: true),
                    HasPendingRestart = table.Column<bool>(nullable: false),
                    HasUpdateAvailable = table.Column<bool>(nullable: false),
                    HttpServerPortNumber = table.Column<int>(nullable: false),
                    HttpsPortNumber = table.Column<int>(nullable: false),
                    InternalMetadataPath = table.Column<string>(nullable: true),
                    IsShuttingDown = table.Column<bool>(nullable: false),
                    ItemsByNamePath = table.Column<string>(nullable: true),
                    LogPath = table.Column<string>(nullable: true),
                    OperatingSystemDisplayName = table.Column<string>(nullable: true),
                    PackageName = table.Column<string>(nullable: true),
                    ProgramDataPath = table.Column<string>(nullable: true),
                    SupportsAutoRunAtStartup = table.Column<bool>(nullable: false),
                    SupportsHttps = table.Column<bool>(nullable: false),
                    SupportsLibraryMonitor = table.Column<bool>(nullable: false),
                    SystemArchitecture = table.Column<int>(nullable: false),
                    SystemUpdateLevel = table.Column<int>(nullable: false),
                    TranscodingTempPath = table.Column<string>(nullable: true),
                    WebSocketPortNumber = table.Column<int>(nullable: false)
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
        }
    }
}
