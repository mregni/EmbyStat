using System;
using Microsoft.EntityFrameworkCore.Migrations;

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
                    ParentId = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    OfficialRating = table.Column<string>(nullable: true),
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
                    Id = table.Column<Guid>(nullable: false),
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
                    Value = table.Column<string>(nullable: true)
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
                    Name = table.Column<string>(nullable: true),
                    LastUserName = table.Column<string>(nullable: true)
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
                name: "EmbyStatus",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmbyStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Media",
                columns: table => new
                {
                    DateCreated = table.Column<DateTime>(nullable: true),
                    Id = table.Column<Guid>(nullable: false),
                    ParentId = table.Column<Guid>(nullable: false),
                    PremiereDate = table.Column<DateTime>(nullable: true),
                    ProductionYear = table.Column<int>(nullable: true),
                    Banner = table.Column<string>(nullable: true),
                    Logo = table.Column<string>(nullable: true),
                    Primary = table.Column<string>(nullable: true),
                    Thumb = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Path = table.Column<string>(nullable: true),
                    SortName = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    CommunityRating = table.Column<float>(nullable: true),
                    RunTimeTicks = table.Column<long>(nullable: true),
                    Overview = table.Column<string>(nullable: true),
                    IMDB = table.Column<string>(nullable: true),
                    TMDB = table.Column<string>(nullable: true),
                    TVDB = table.Column<string>(nullable: true),
                    Container = table.Column<string>(nullable: true),
                    HasSubtitles = table.Column<bool>(nullable: true),
                    IdHD = table.Column<bool>(nullable: true),
                    MediaType = table.Column<string>(nullable: true),
                    DvdEpisodeNumber = table.Column<float>(nullable: true),
                    DvdSeasonNumber = table.Column<int>(nullable: true),
                    IndexNumber = table.Column<int>(nullable: true),
                    IndexNumberEnd = table.Column<int>(nullable: true),
                    HomePageUrl = table.Column<string>(nullable: true),
                    OfficialRating = table.Column<string>(nullable: true),
                    OriginalTitle = table.Column<string>(nullable: true),
                    CumulativeRunTimeTicks = table.Column<long>(nullable: true),
                    DateLastMediaAdded = table.Column<DateTime>(nullable: true),
                    Show_HomePageUrl = table.Column<string>(nullable: true),
                    Show_OfficialRating = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    TvdbSynced = table.Column<bool>(nullable: true),
                    MissingEpisodesCount = table.Column<int>(nullable: true),
                    TvdbFailed = table.Column<bool>(nullable: true),
                    Season_IndexNumber = table.Column<int>(nullable: true),
                    Season_IndexNumberEnd = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Media", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Synced = table.Column<bool>(nullable: false),
                    ChildCount = table.Column<int>(nullable: false),
                    EpisodeCount = table.Column<int>(nullable: false),
                    Etag = table.Column<string>(nullable: true),
                    HomePageUrl = table.Column<string>(nullable: true),
                    MovieCount = table.Column<int>(nullable: false),
                    OverView = table.Column<string>(nullable: true),
                    BirthDate = table.Column<DateTime>(nullable: true),
                    IMDB = table.Column<string>(nullable: true),
                    TMDB = table.Column<string>(nullable: true),
                    SeriesCount = table.Column<int>(nullable: false),
                    SortName = table.Column<string>(nullable: true),
                    Primary = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Plugins",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: true),
                    Version = table.Column<string>(nullable: true),
                    ConfigurationFileName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Id = table.Column<string>(nullable: false),
                    ImageUrl = table.Column<string>(nullable: true)
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
                    SystemUpdateLevel = table.Column<string>(nullable: true),
                    OperatingSystemDisplayName = table.Column<string>(nullable: true),
                    HasPendingRestart = table.Column<bool>(nullable: false),
                    IsShuttingDown = table.Column<bool>(nullable: false),
                    SupportsLibraryMonitor = table.Column<bool>(nullable: false),
                    WebSocketPortNumber = table.Column<int>(nullable: false),
                    CanSelfRestart = table.Column<bool>(nullable: false),
                    CanSelfUpdate = table.Column<bool>(nullable: false),
                    CanLaunchWebBrowser = table.Column<bool>(nullable: false),
                    ProgramDataPath = table.Column<string>(nullable: true),
                    ItemsByNamePath = table.Column<string>(nullable: true),
                    CachePath = table.Column<string>(nullable: true),
                    LogPath = table.Column<string>(nullable: true),
                    InternalMetadataPath = table.Column<string>(nullable: true),
                    TranscodingTempPath = table.Column<string>(nullable: true),
                    HttpServerPortNumber = table.Column<int>(nullable: false),
                    SupportsHttps = table.Column<bool>(nullable: false),
                    HttpsPortNumber = table.Column<int>(nullable: false),
                    HasUpdateAvailable = table.Column<bool>(nullable: false),
                    SupportsAutoRunAtStartup = table.Column<bool>(nullable: false),
                    EncoderLocationType = table.Column<string>(nullable: true),
                    SystemArchitecture = table.Column<string>(nullable: true),
                    LocalAddress = table.Column<string>(nullable: true),
                    WanAddress = table.Column<string>(nullable: true),
                    ServerName = table.Column<string>(nullable: true),
                    Version = table.Column<string>(nullable: true),
                    OperatingSystem = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Statistics",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CalculationDateTime = table.Column<DateTime>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    JsonResult = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statistics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskResults",
                columns: table => new
                {
                    StartTimeUtc = table.Column<DateTime>(nullable: false),
                    EndTimeUtc = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Key = table.Column<string>(nullable: true),
                    Id = table.Column<string>(nullable: false),
                    ErrorMessage = table.Column<string>(nullable: true),
                    LongErrorMessage = table.Column<string>(nullable: true)
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
                    TaskKey = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    TimeOfDayTicks = table.Column<long>(nullable: true),
                    IntervalTicks = table.Column<long>(nullable: true),
                    DayOfWeek = table.Column<int>(nullable: true),
                    MaxRuntimeTicks = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskTriggerInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    LastActivityDate = table.Column<DateTime>(nullable: true),
                    LastLoginDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    IsAdmin = table.Column<bool>(nullable: false),
                    IsHidden = table.Column<bool>(nullable: false),
                    IsDisabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
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
                    VideoId = table.Column<Guid>(nullable: false)
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
                name: "MediaCollection",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    MediaId = table.Column<Guid>(nullable: false),
                    CollectionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaCollection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaCollection_Collections_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MediaCollection_Media_MediaId",
                        column: x => x.MediaId,
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MediaGenres",
                columns: table => new
                {
                    MediaId = table.Column<Guid>(nullable: false),
                    GenreId = table.Column<Guid>(nullable: false)
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
                    VideoType = table.Column<string>(nullable: true),
                    VideoId = table.Column<Guid>(nullable: false)
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
                name: "SeasonEpisodes",
                columns: table => new
                {
                    SeasonId = table.Column<Guid>(nullable: false),
                    EpisodeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeasonEpisodes", x => new { x.EpisodeId, x.SeasonId });
                    table.ForeignKey(
                        name: "FK_SeasonEpisodes_Media_EpisodeId",
                        column: x => x.EpisodeId,
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SeasonEpisodes_Media_SeasonId",
                        column: x => x.SeasonId,
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
                    VideoId = table.Column<Guid>(nullable: false)
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
                    Width = table.Column<int>(nullable: true),
                    VideoId = table.Column<Guid>(nullable: false)
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
                    Type = table.Column<string>(nullable: true),
                    ExtraId = table.Column<Guid>(nullable: false),
                    PersonId = table.Column<Guid>(nullable: false)
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
                name: "StatisticCollection",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    StatisticId = table.Column<Guid>(nullable: false),
                    CollectionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatisticCollection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StatisticCollection_Collections_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StatisticCollection_Statistics_StatisticId",
                        column: x => x.StatisticId,
                        principalTable: "Statistics",
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
                name: "IX_MediaCollection_CollectionId",
                table: "MediaCollection",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaCollection_MediaId",
                table: "MediaCollection",
                column: "MediaId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaGenres_MediaId",
                table: "MediaGenres",
                column: "MediaId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaSources_VideoId",
                table: "MediaSources",
                column: "VideoId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonEpisodes_SeasonId",
                table: "SeasonEpisodes",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_StatisticCollection_CollectionId",
                table: "StatisticCollection",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_StatisticCollection_StatisticId",
                table: "StatisticCollection",
                column: "StatisticId");

            migrationBuilder.CreateIndex(
                name: "IX_SubtitleStreams_VideoId",
                table: "SubtitleStreams",
                column: "VideoId");

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
                name: "Configuration");

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "Drives");

            migrationBuilder.DropTable(
                name: "EmbyStatus");

            migrationBuilder.DropTable(
                name: "ExtraPersons");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropTable(
                name: "MediaCollection");

            migrationBuilder.DropTable(
                name: "MediaGenres");

            migrationBuilder.DropTable(
                name: "MediaSources");

            migrationBuilder.DropTable(
                name: "Plugins");

            migrationBuilder.DropTable(
                name: "SeasonEpisodes");

            migrationBuilder.DropTable(
                name: "ServerInfo");

            migrationBuilder.DropTable(
                name: "StatisticCollection");

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
                name: "Collections");

            migrationBuilder.DropTable(
                name: "Statistics");

            migrationBuilder.DropTable(
                name: "Media");
        }
    }
}
