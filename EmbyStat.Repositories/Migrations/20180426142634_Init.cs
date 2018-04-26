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
                name: "Boxsets",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    OfficialRating = table.Column<string>(nullable: true),
                    ParentId = table.Column<string>(nullable: false),
                    Primary = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boxsets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Collections",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    PrimaryImage = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Configuration",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AccessToken = table.Column<string>(nullable: true),
                    EmbyServerAddress = table.Column<string>(nullable: true),
                    EmbyUserId = table.Column<string>(nullable: true),
                    EmbyUserName = table.Column<string>(nullable: true),
                    Language = table.Column<string>(nullable: false),
                    ServerName = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true),
                    WizardFinished = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configuration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AppName = table.Column<string>(nullable: true),
                    AppVersion = table.Column<string>(nullable: true),
                    LastUserName = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
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
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Media",
                columns: table => new
                {
                    DvdEpisodeNumber = table.Column<float>(nullable: true),
                    DvdSeasonNumber = table.Column<int>(nullable: true),
                    IndexNumber = table.Column<int>(nullable: true),
                    IndexNumberEnd = table.Column<int>(nullable: true),
                    CommunityRating = table.Column<float>(nullable: true),
                    IMDB = table.Column<string>(nullable: true),
                    Overview = table.Column<string>(nullable: true),
                    RunTimeTicks = table.Column<long>(nullable: true),
                    TMDB = table.Column<string>(nullable: true),
                    TVDB = table.Column<string>(nullable: true),
                    Id = table.Column<string>(nullable: false),
                    Banner = table.Column<string>(nullable: true),
                    CollectionId = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    Logo = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    ParentId = table.Column<string>(nullable: false),
                    Path = table.Column<string>(nullable: true),
                    PremiereDate = table.Column<DateTime>(nullable: true),
                    Primary = table.Column<string>(nullable: true),
                    ProductionYear = table.Column<int>(nullable: true),
                    SortName = table.Column<string>(nullable: true),
                    Thumb = table.Column<string>(nullable: true),
                    Container = table.Column<string>(nullable: true),
                    HasSubtitles = table.Column<bool>(nullable: true),
                    IdHD = table.Column<bool>(nullable: true),
                    MediaType = table.Column<string>(nullable: true),
                    HomePageUrl = table.Column<string>(nullable: true),
                    OfficialRating = table.Column<string>(nullable: true),
                    OriginalTitle = table.Column<string>(nullable: true),
                    Season_IndexNumber = table.Column<int>(nullable: true),
                    Season_IndexNumberEnd = table.Column<int>(nullable: true),
                    CumulativeRunTimeTicks = table.Column<long>(nullable: true),
                    DateLastMediaAdded = table.Column<DateTime>(nullable: true),
                    Show_HomePageUrl = table.Column<string>(nullable: true),
                    Show_OfficialRating = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Media", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.Id);
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
                name: "Servers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CanSelfRestart = table.Column<bool>(nullable: false),
                    CanSelfUpdate = table.Column<bool>(nullable: false),
                    HasPendingRestart = table.Column<bool>(nullable: false),
                    HasUpdateAvailable = table.Column<bool>(nullable: false),
                    HttpServerPortNumber = table.Column<int>(nullable: false),
                    HttpsPortNumber = table.Column<int>(nullable: false),
                    LocalAddress = table.Column<string>(nullable: true),
                    MacAddress = table.Column<string>(nullable: true),
                    OperatingSystem = table.Column<string>(nullable: true),
                    OperatingSystemDispayName = table.Column<string>(nullable: true),
                    ServerName = table.Column<string>(nullable: true),
                    SystemArchitecture = table.Column<string>(nullable: true),
                    SystemUpdateLevel = table.Column<string>(nullable: true),
                    Version = table.Column<string>(nullable: true),
                    WanAddress = table.Column<string>(nullable: true),
                    WebSocketPortNumber = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servers", x => x.Id);
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

            migrationBuilder.CreateTable(
                name: "AudioStreams",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    BitRate = table.Column<long>(nullable: true),
                    ChannelLayout = table.Column<string>(nullable: true),
                    Channels = table.Column<int>(nullable: true),
                    Codec = table.Column<string>(nullable: true),
                    Language = table.Column<string>(nullable: true),
                    SampleRate = table.Column<int>(nullable: true),
                    VideoId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AudioStreams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AudioStreams_Media_VideoId",
                        column: x => x.VideoId,
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MediaGenres",
                columns: table => new
                {
                    GenreId = table.Column<string>(nullable: false),
                    MediaId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaGenres", x => new { x.GenreId, x.MediaId });
                    table.ForeignKey(
                        name: "FK_MediaGenres_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MediaGenres_Media_MediaId",
                        column: x => x.MediaId,
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MediaSources",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    BitRate = table.Column<long>(nullable: true),
                    Container = table.Column<string>(nullable: true),
                    Path = table.Column<string>(nullable: true),
                    Protocol = table.Column<string>(nullable: true),
                    RunTimeTicks = table.Column<long>(nullable: true),
                    VideoId = table.Column<string>(nullable: true),
                    VideoType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaSources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaSources_Media_VideoId",
                        column: x => x.VideoId,
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubtitleStreams",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Codec = table.Column<string>(nullable: true),
                    DisplayTitle = table.Column<string>(nullable: true),
                    IsDefault = table.Column<bool>(nullable: false),
                    Language = table.Column<string>(nullable: true),
                    VideoId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubtitleStreams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubtitleStreams_Media_VideoId",
                        column: x => x.VideoId,
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VideoStreams",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AspectRatio = table.Column<string>(nullable: true),
                    AverageFrameRate = table.Column<float>(nullable: true),
                    BitRate = table.Column<long>(nullable: true),
                    Channels = table.Column<int>(nullable: true),
                    Height = table.Column<int>(nullable: true),
                    Language = table.Column<string>(nullable: true),
                    VideoId = table.Column<string>(nullable: true),
                    Width = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoStreams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideoStreams_Media_VideoId",
                        column: x => x.VideoId,
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExtraPersons",
                columns: table => new
                {
                    ExtraId = table.Column<string>(nullable: false),
                    PersonId = table.Column<string>(nullable: false),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtraPersons", x => new { x.ExtraId, x.PersonId });
                    table.ForeignKey(
                        name: "FK_ExtraPersons_Media_ExtraId",
                        column: x => x.ExtraId,
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExtraPersons_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    IsAdmin = table.Column<bool>(nullable: false),
                    IsDisabled = table.Column<bool>(nullable: false),
                    IsHidden = table.Column<bool>(nullable: false),
                    LastActivityDate = table.Column<DateTime>(nullable: true),
                    LastLoginDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    ServerId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Servers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Servers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AudioStreams_VideoId",
                table: "AudioStreams",
                column: "VideoId");

            migrationBuilder.CreateIndex(
                name: "IX_ExtraPersons_PersonId",
                table: "ExtraPersons",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaGenres_MediaId",
                table: "MediaGenres",
                column: "MediaId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaSources_VideoId",
                table: "MediaSources",
                column: "VideoId");

            migrationBuilder.CreateIndex(
                name: "IX_SubtitleStreams_VideoId",
                table: "SubtitleStreams",
                column: "VideoId");

            migrationBuilder.CreateIndex(
                name: "IX_User_ServerId",
                table: "User",
                column: "ServerId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoStreams_VideoId",
                table: "VideoStreams",
                column: "VideoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AudioStreams");

            migrationBuilder.DropTable(
                name: "Boxsets");

            migrationBuilder.DropTable(
                name: "Collections");

            migrationBuilder.DropTable(
                name: "Configuration");

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "Drives");

            migrationBuilder.DropTable(
                name: "ExtraPersons");

            migrationBuilder.DropTable(
                name: "MediaGenres");

            migrationBuilder.DropTable(
                name: "MediaSources");

            migrationBuilder.DropTable(
                name: "Plugins");

            migrationBuilder.DropTable(
                name: "ServerInfo");

            migrationBuilder.DropTable(
                name: "SubtitleStreams");

            migrationBuilder.DropTable(
                name: "TaskResults");

            migrationBuilder.DropTable(
                name: "TaskTriggerInfos");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "VideoStreams");

            migrationBuilder.DropTable(
                name: "People");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "Servers");

            migrationBuilder.DropTable(
                name: "Media");
        }
    }
}
