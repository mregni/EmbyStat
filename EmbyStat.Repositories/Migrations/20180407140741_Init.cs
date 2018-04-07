using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace EmbyStat.Repositories.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Configuration",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AccessToken = table.Column<string>(nullable: true),
                    EmbyServerAddress = table.Column<string>(nullable: true),
                    EmbyUserName = table.Column<string>(nullable: true),
                    Language = table.Column<string>(nullable: false),
                    ServerName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true),
                    WizardFinished = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configuration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Drives",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Path = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drives", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "TaskResults",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    EndTimeUtc = table.Column<DateTime>(nullable: false),
                    ErrorMessage = table.Column<string>(nullable: true),
                    Key = table.Column<string>(nullable: true),
                    LongErrorMessage = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    StartTimeUtc = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskResults", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskTriggerInfos",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DayOfWeek = table.Column<int>(nullable: true),
                    IntervalTicks = table.Column<long>(nullable: true),
                    MaxRuntimeTicks = table.Column<long>(nullable: true),
                    TaskKey = table.Column<string>(nullable: true),
                    TimeOfDayTicks = table.Column<long>(nullable: true),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskTriggerInfos", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Configuration");

            migrationBuilder.DropTable(
                name: "Drives");

            migrationBuilder.DropTable(
                name: "Plugins");

            migrationBuilder.DropTable(
                name: "ServerInfo");

            migrationBuilder.DropTable(
                name: "TaskResults");

            migrationBuilder.DropTable(
                name: "TaskTriggerInfos");
        }
    }
}
