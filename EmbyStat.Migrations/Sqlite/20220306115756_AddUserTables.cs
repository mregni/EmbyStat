using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmbyStat.Migrations.Sqlite
{
    public partial class AddUserTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageTag",
                table: "Plugins");

            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    LastUserName = table.Column<string>(type: "TEXT", nullable: true),
                    AppName = table.Column<string>(type: "TEXT", nullable: true),
                    AppVersion = table.Column<string>(type: "TEXT", nullable: true),
                    LastUserId = table.Column<string>(type: "TEXT", nullable: true),
                    DateLastActivity = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    IconUrl = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: true),
                    PlayDefaultAudioTrack = table.Column<bool>(type: "INTEGER", nullable: false),
                    SubtitleLanguagePreference = table.Column<string>(type: "TEXT", nullable: true),
                    DisplayMissingEpisodes = table.Column<bool>(type: "INTEGER", nullable: false),
                    SubtitleMode = table.Column<string>(type: "TEXT", nullable: true),
                    EnableLocalPassword = table.Column<bool>(type: "INTEGER", nullable: false),
                    HidePlayedInLatest = table.Column<bool>(type: "INTEGER", nullable: false),
                    RememberAudioSelections = table.Column<bool>(type: "INTEGER", nullable: false),
                    RememberSubtitleSelections = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnableNextEpisodeAutoPlay = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserPolicies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: true),
                    IsAdministrator = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsHidden = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsHiddenRemotely = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsHiddenFromUnusedDevices = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDisabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsTagBlockingModeInclusive = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnableUserPreferenceAccess = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnableRemoteControlOfOtherUsers = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnableSharedDeviceControl = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnableRemoteAccess = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnableLiveTvManagement = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnableLiveTvAccess = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnableMediaPlayback = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnableAudioPlaybackTranscoding = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnableVideoPlaybackTranscoding = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnablePlaybackRemuxing = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnableContentDeletion = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnableContentDownloading = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnableSubtitleDownloading = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnableSubtitleManagement = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnableSyncTranscoding = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnableMediaConversion = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnableAllChannels = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnableAllFolders = table.Column<bool>(type: "INTEGER", nullable: false),
                    InvalidLoginAttemptCount = table.Column<int>(type: "INTEGER", nullable: false),
                    EnablePublicSharing = table.Column<bool>(type: "INTEGER", nullable: false),
                    RemoteClientBitrateLimit = table.Column<int>(type: "INTEGER", nullable: false),
                    AuthenticationProviderId = table.Column<string>(type: "TEXT", nullable: true),
                    SimultaneousStreamLimit = table.Column<int>(type: "INTEGER", nullable: false),
                    EnableAllDevices = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPolicies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    ServerId = table.Column<string>(type: "TEXT", nullable: true),
                    HasPassword = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasConfiguredPassword = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasConfiguredEasyPassword = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastLoginDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LastActivityDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    ConfigurationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PolicyId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_UserConfigurations_ConfigurationId",
                        column: x => x.ConfigurationId,
                        principalTable: "UserConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_UserPolicies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "UserPolicies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserConfigurations_UserId",
                table: "UserConfigurations",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPolicies_UserId",
                table: "UserPolicies",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_ConfigurationId",
                table: "Users",
                column: "ConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PolicyId",
                table: "Users",
                column: "PolicyId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserConfigurations_Users_UserId",
                table: "UserConfigurations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPolicies_Users_UserId",
                table: "UserPolicies",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_UserConfigurations_ConfigurationId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPolicies_Users_UserId",
                table: "UserPolicies");

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "UserConfigurations");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "UserPolicies");

            migrationBuilder.AddColumn<string>(
                name: "ImageTag",
                table: "Plugins",
                type: "TEXT",
                nullable: true);
        }
    }
}
