using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmbyStat.Migrations.Sqlite
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    DateLastActivity = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IconUrl = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Filters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Field = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Values = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Filters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentProgressPercentage = table.Column<double>(type: "REAL", nullable: true),
                    StartTimeUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EndTimeUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Trigger = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Code = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Libraries",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Primary = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Sync = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastSynced = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Libraries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MediaServerInfo",
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
                    table.PrimaryKey("PK_MediaServerInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MediaServerStatus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MissedPings = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaServerStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Primary = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Plugins",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Version = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plugins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    NormalizedName = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Statistics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CalculationDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    JsonResult = table.Column<string>(type: "TEXT", nullable: true),
                    IsValid = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statistics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderKey = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    RefreshTokens = table.Column<string>(type: "TEXT", nullable: true),
                    UserName = table.Column<string>(type: "TEXT", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    OriginalTitle = table.Column<string>(type: "TEXT", nullable: true),
                    LibraryId = table.Column<string>(type: "TEXT", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Banner = table.Column<string>(type: "TEXT", nullable: true),
                    Logo = table.Column<string>(type: "TEXT", nullable: true),
                    Primary = table.Column<string>(type: "TEXT", nullable: true),
                    Thumb = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Path = table.Column<string>(type: "TEXT", nullable: true),
                    PremiereDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ProductionYear = table.Column<int>(type: "INTEGER", nullable: true),
                    SortName = table.Column<string>(type: "TEXT", nullable: true),
                    CommunityRating = table.Column<decimal>(type: "TEXT", nullable: true),
                    IMDB = table.Column<string>(type: "TEXT", nullable: true),
                    TMDB = table.Column<int>(type: "INTEGER", nullable: true),
                    TVDB = table.Column<string>(type: "TEXT", nullable: true),
                    RunTimeTicks = table.Column<long>(type: "INTEGER", nullable: true),
                    OfficialRating = table.Column<string>(type: "TEXT", nullable: true),
                    Container = table.Column<string>(type: "TEXT", nullable: true),
                    Video3DFormat = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Movies_Libraries_LibraryId",
                        column: x => x.LibraryId,
                        principalTable: "Libraries",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Shows",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    CumulativeRunTimeTicks = table.Column<long>(type: "INTEGER", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    ExternalSynced = table.Column<bool>(type: "INTEGER", nullable: false),
                    SizeInMb = table.Column<double>(type: "REAL", nullable: false),
                    LibraryId = table.Column<string>(type: "TEXT", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Banner = table.Column<string>(type: "TEXT", nullable: true),
                    Logo = table.Column<string>(type: "TEXT", nullable: true),
                    Primary = table.Column<string>(type: "TEXT", nullable: true),
                    Thumb = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Path = table.Column<string>(type: "TEXT", nullable: true),
                    PremiereDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ProductionYear = table.Column<int>(type: "INTEGER", nullable: true),
                    SortName = table.Column<string>(type: "TEXT", nullable: true),
                    CommunityRating = table.Column<decimal>(type: "TEXT", nullable: true),
                    IMDB = table.Column<string>(type: "TEXT", nullable: true),
                    TMDB = table.Column<int>(type: "INTEGER", nullable: true),
                    TVDB = table.Column<string>(type: "TEXT", nullable: true),
                    RunTimeTicks = table.Column<long>(type: "INTEGER", nullable: true),
                    OfficialRating = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shows_Libraries_LibraryId",
                        column: x => x.LibraryId,
                        principalTable: "Libraries",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GenreMovie",
                columns: table => new
                {
                    GenresId = table.Column<string>(type: "TEXT", nullable: false),
                    MoviesId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenreMovie", x => new { x.GenresId, x.MoviesId });
                    table.ForeignKey(
                        name: "FK_GenreMovie_Genres_GenresId",
                        column: x => x.GenresId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenreMovie_Movies_MoviesId",
                        column: x => x.MoviesId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GenreShow",
                columns: table => new
                {
                    GenresId = table.Column<string>(type: "TEXT", nullable: false),
                    ShowsId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenreShow", x => new { x.GenresId, x.ShowsId });
                    table.ForeignKey(
                        name: "FK_GenreShow_Genres_GenresId",
                        column: x => x.GenresId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenreShow_Shows_ShowsId",
                        column: x => x.ShowsId,
                        principalTable: "Shows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MediaPerson",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    MovieId = table.Column<string>(type: "TEXT", nullable: true),
                    ShowId = table.Column<string>(type: "TEXT", nullable: true),
                    PersonId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaPerson", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaPerson_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MediaPerson_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MediaPerson_Shows_ShowId",
                        column: x => x.ShowId,
                        principalTable: "Shows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Season",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    IndexNumber = table.Column<int>(type: "INTEGER", nullable: true),
                    IndexNumberEnd = table.Column<int>(type: "INTEGER", nullable: true),
                    LocationType = table.Column<int>(type: "INTEGER", nullable: false),
                    ShowId = table.Column<string>(type: "TEXT", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Banner = table.Column<string>(type: "TEXT", nullable: true),
                    Logo = table.Column<string>(type: "TEXT", nullable: true),
                    Primary = table.Column<string>(type: "TEXT", nullable: true),
                    Thumb = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Path = table.Column<string>(type: "TEXT", nullable: true),
                    PremiereDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ProductionYear = table.Column<int>(type: "INTEGER", nullable: true),
                    SortName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Season", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Season_Shows_ShowId",
                        column: x => x.ShowId,
                        principalTable: "Shows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Episodes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    DvdEpisodeNumber = table.Column<float>(type: "REAL", nullable: true),
                    DvdSeasonNumber = table.Column<int>(type: "INTEGER", nullable: true),
                    IndexNumber = table.Column<int>(type: "INTEGER", nullable: true),
                    IndexNumberEnd = table.Column<int>(type: "INTEGER", nullable: true),
                    LocationType = table.Column<int>(type: "INTEGER", nullable: false),
                    SeasonId = table.Column<string>(type: "TEXT", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Banner = table.Column<string>(type: "TEXT", nullable: true),
                    Logo = table.Column<string>(type: "TEXT", nullable: true),
                    Primary = table.Column<string>(type: "TEXT", nullable: true),
                    Thumb = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Path = table.Column<string>(type: "TEXT", nullable: true),
                    PremiereDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ProductionYear = table.Column<int>(type: "INTEGER", nullable: true),
                    SortName = table.Column<string>(type: "TEXT", nullable: true),
                    CommunityRating = table.Column<decimal>(type: "TEXT", nullable: true),
                    IMDB = table.Column<string>(type: "TEXT", nullable: true),
                    TMDB = table.Column<int>(type: "INTEGER", nullable: true),
                    TVDB = table.Column<string>(type: "TEXT", nullable: true),
                    RunTimeTicks = table.Column<long>(type: "INTEGER", nullable: true),
                    OfficialRating = table.Column<string>(type: "TEXT", nullable: true),
                    Container = table.Column<string>(type: "TEXT", nullable: true),
                    Video3DFormat = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Episodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Episodes_Season_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Season",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AudioStream",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    BitRate = table.Column<int>(type: "INTEGER", nullable: true),
                    ChannelLayout = table.Column<string>(type: "TEXT", nullable: true),
                    Channels = table.Column<int>(type: "INTEGER", nullable: true),
                    Codec = table.Column<string>(type: "TEXT", nullable: true),
                    Language = table.Column<string>(type: "TEXT", nullable: true),
                    SampleRate = table.Column<int>(type: "INTEGER", nullable: true),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    MovieId = table.Column<string>(type: "TEXT", nullable: true),
                    EpisodeId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AudioStream", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AudioStream_Episodes_EpisodeId",
                        column: x => x.EpisodeId,
                        principalTable: "Episodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AudioStream_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MediaSource",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    BitRate = table.Column<int>(type: "INTEGER", nullable: true),
                    Container = table.Column<string>(type: "TEXT", nullable: true),
                    Path = table.Column<string>(type: "TEXT", nullable: true),
                    Protocol = table.Column<string>(type: "TEXT", nullable: true),
                    RunTimeTicks = table.Column<long>(type: "INTEGER", nullable: true),
                    SizeInMb = table.Column<double>(type: "REAL", nullable: false),
                    MovieId = table.Column<string>(type: "TEXT", nullable: true),
                    EpisodeId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaSource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaSource_Episodes_EpisodeId",
                        column: x => x.EpisodeId,
                        principalTable: "Episodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MediaSource_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubtitleStream",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Codec = table.Column<string>(type: "TEXT", nullable: true),
                    DisplayTitle = table.Column<string>(type: "TEXT", nullable: true),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    Language = table.Column<string>(type: "TEXT", nullable: true),
                    MovieId = table.Column<string>(type: "TEXT", nullable: true),
                    EpisodeId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubtitleStream", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubtitleStream_Episodes_EpisodeId",
                        column: x => x.EpisodeId,
                        principalTable: "Episodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubtitleStream_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VideoStream",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    AspectRatio = table.Column<string>(type: "TEXT", nullable: true),
                    AverageFrameRate = table.Column<float>(type: "REAL", nullable: true),
                    BitRate = table.Column<int>(type: "INTEGER", nullable: true),
                    Channels = table.Column<int>(type: "INTEGER", nullable: true),
                    Height = table.Column<int>(type: "INTEGER", nullable: true),
                    Language = table.Column<string>(type: "TEXT", nullable: true),
                    Width = table.Column<int>(type: "INTEGER", nullable: true),
                    BitDepth = table.Column<int>(type: "INTEGER", nullable: true),
                    Codec = table.Column<string>(type: "TEXT", nullable: true),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    VideoRange = table.Column<string>(type: "TEXT", nullable: true),
                    MovieId = table.Column<string>(type: "TEXT", nullable: true),
                    EpisodeId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoStream", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideoStream_Episodes_EpisodeId",
                        column: x => x.EpisodeId,
                        principalTable: "Episodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VideoStream_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MediaServerUserConfiguration",
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
                    table.PrimaryKey("PK_MediaServerUserConfiguration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MediaServerUserPolicy",
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
                    table.PrimaryKey("PK_MediaServerUserPolicy", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MediaServerUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    ServerId = table.Column<string>(type: "TEXT", nullable: true),
                    HasPassword = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasConfiguredPassword = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasConfiguredEasyPassword = table.Column<bool>(type: "INTEGER", nullable: false),
                    PrimaryImageTag = table.Column<string>(type: "TEXT", nullable: true),
                    LastLoginDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LastActivityDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    ConfigurationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PolicyId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaServerUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaServerUsers_MediaServerUserConfiguration_ConfigurationId",
                        column: x => x.ConfigurationId,
                        principalTable: "MediaServerUserConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MediaServerUsers_MediaServerUserPolicy_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "MediaServerUserPolicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Jobs",
                columns: new[] { "Id", "CurrentProgressPercentage", "Description", "EndTimeUtc", "StartTimeUtc", "State", "Title", "Trigger" },
                values: new object[] { new Guid("41e0bf22-1e6b-4f5d-90be-ec966f746a2f"), 0.0, "SYSTEM-SYNCDESCRIPTION", null, null, 0, "SYSTEM-SYNC", "0 2 * * *" });

            migrationBuilder.InsertData(
                table: "Jobs",
                columns: new[] { "Id", "CurrentProgressPercentage", "Description", "EndTimeUtc", "StartTimeUtc", "State", "Title", "Trigger" },
                values: new object[] { new Guid("78bc2bf0-abd9-48ef-aeff-9c396d644f2a"), 0.0, "UPDATE-CHECKERDESCRIPTION", null, null, 0, "UPDATE-CHECKER", "0 */12 * * *" });

            migrationBuilder.InsertData(
                table: "Jobs",
                columns: new[] { "Id", "CurrentProgressPercentage", "Description", "EndTimeUtc", "StartTimeUtc", "State", "Title", "Trigger" },
                values: new object[] { new Guid("b109ca73-0563-4062-a3e2-f7e6a00b73e9"), 0.0, "DATABASE-CLEANUPDESCRIPTION", null, null, 0, "DATABASE-CLEANUP", "0 4 * * *" });

            migrationBuilder.InsertData(
                table: "Jobs",
                columns: new[] { "Id", "CurrentProgressPercentage", "Description", "EndTimeUtc", "StartTimeUtc", "State", "Title", "Trigger" },
                values: new object[] { new Guid("be68900b-ee1d-41ef-b12f-60ef3106052e"), 0.0, "SHOW-SYNCDESCRIPTION", null, null, 0, "SHOW-SYNC", "0 3 * * *" });

            migrationBuilder.InsertData(
                table: "Jobs",
                columns: new[] { "Id", "CurrentProgressPercentage", "Description", "EndTimeUtc", "StartTimeUtc", "State", "Title", "Trigger" },
                values: new object[] { new Guid("c40555dc-ea57-4c6e-a225-905223d31c3c"), 0.0, "MOVIE-SYNCDESCRIPTION", null, null, 0, "MOVIE-SYNC", "0 2 * * *" });

            migrationBuilder.InsertData(
                table: "Jobs",
                columns: new[] { "Id", "CurrentProgressPercentage", "Description", "EndTimeUtc", "StartTimeUtc", "State", "Title", "Trigger" },
                values: new object[] { new Guid("ce1fbc9e-21ee-450b-9cdf-58a0e17ea98e"), 0.0, "PINGDESCRIPTION", null, null, 0, "PING", "*/5 * * * *" });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[] { "082d8aaf-f86a-4401-bf0f-c315b3c9d904", "nl-NL", "Nederlands" });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[] { "282182b9-9332-4266-a093-5ff5b7f927a9", "it-IT", "Italiano" });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[] { "3e8d27e9-e314-4d57-967f-cf5d84144acf", "sv-SE", "Svenska" });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[] { "490c1cb5-b711-4514-aa97-d22ddff2b2fa", "pt-PT", "Português" });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[] { "62fe1b5e-3328-450b-b24b-fa16bea58870", "en-US", "English" });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[] { "6b103b14-20d1-49c0-b7ce-8d701399b64d", "ro-RO", "Românesc" });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[] { "91cca672-af55-4d55-899a-798826a43773", "fi-FI", "Suomi" });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[] { "97616a9b-60f9-407a-9a87-b4518da5e5f4", "cs-CZ", "简体中文" });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[] { "99142c2f-379e-4a25-879b-ecfe25ee9e7c", "fr-FR", "Français" });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[] { "a48a2ef9-3b64-4069-8e31-252abb6d07a3", "hu-HU", "Magyar" });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[] { "b21074db-74b9-4e24-8867-34e82c265256", "pt-BR", "Brasileiro" });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[] { "c0a60a3b-282e-46f5-aa7f-661c88f2edb0", "es-ES", "Español" });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[] { "c9df5e5c-75f3-46c2-8095-97fde33531d8", "de-DE", "Deutsch" });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[] { "d6401f6b-becf-49e2-b82e-1018b3bf607f", "el-GR", "Ελληνικά" });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[] { "d8b0ae7b-9ba7-4a51-9d7c-94402b51265d", "no-NO", "Norsk" });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[] { "e1940bfd-00c4-46f9-b9ac-a87a5d92e8ca", "da-DK", "Dansk" });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[] { "f3966f43-3ec6-456e-850f-a2ebfc0b539b", "pl-PL", "Polski" });

            migrationBuilder.InsertData(
                table: "MediaServerStatus",
                columns: new[] { "Id", "MissedPings" },
                values: new object[] { new Guid("e55668a1-6a81-47ba-882d-738347e7e9cd"), 0 });

            migrationBuilder.CreateIndex(
                name: "IX_AudioStream_EpisodeId",
                table: "AudioStream",
                column: "EpisodeId");

            migrationBuilder.CreateIndex(
                name: "IX_AudioStream_MovieId",
                table: "AudioStream",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_Episodes_SeasonId",
                table: "Episodes",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_GenreMovie_MoviesId",
                table: "GenreMovie",
                column: "MoviesId");

            migrationBuilder.CreateIndex(
                name: "IX_Genres_Name",
                table: "Genres",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_GenreShow_ShowsId",
                table: "GenreShow",
                column: "ShowsId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaPerson_MovieId",
                table: "MediaPerson",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaPerson_PersonId",
                table: "MediaPerson",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaPerson_ShowId",
                table: "MediaPerson",
                column: "ShowId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaServerUserConfiguration_UserId",
                table: "MediaServerUserConfiguration",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MediaServerUserPolicy_UserId",
                table: "MediaServerUserPolicy",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MediaServerUsers_ConfigurationId",
                table: "MediaServerUsers",
                column: "ConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaServerUsers_PolicyId",
                table: "MediaServerUsers",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaSource_EpisodeId",
                table: "MediaSource",
                column: "EpisodeId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaSource_MovieId",
                table: "MediaSource",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaSource_SizeInMb",
                table: "MediaSource",
                column: "SizeInMb");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_CommunityRating",
                table: "Movies",
                column: "CommunityRating");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_LibraryId",
                table: "Movies",
                column: "LibraryId");

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
                name: "IX_Season_ShowId",
                table: "Season",
                column: "ShowId");

            migrationBuilder.CreateIndex(
                name: "IX_Shows_CommunityRating",
                table: "Shows",
                column: "CommunityRating");

            migrationBuilder.CreateIndex(
                name: "IX_Shows_LibraryId",
                table: "Shows",
                column: "LibraryId");

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
                name: "IX_SubtitleStream_EpisodeId",
                table: "SubtitleStream",
                column: "EpisodeId");

            migrationBuilder.CreateIndex(
                name: "IX_SubtitleStream_Language",
                table: "SubtitleStream",
                column: "Language");

            migrationBuilder.CreateIndex(
                name: "IX_SubtitleStream_MovieId",
                table: "SubtitleStream",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoStream_AverageFrameRate",
                table: "VideoStream",
                column: "AverageFrameRate");

            migrationBuilder.CreateIndex(
                name: "IX_VideoStream_BitDepth",
                table: "VideoStream",
                column: "BitDepth");

            migrationBuilder.CreateIndex(
                name: "IX_VideoStream_Codec",
                table: "VideoStream",
                column: "Codec");

            migrationBuilder.CreateIndex(
                name: "IX_VideoStream_EpisodeId",
                table: "VideoStream",
                column: "EpisodeId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoStream_Height",
                table: "VideoStream",
                column: "Height");

            migrationBuilder.CreateIndex(
                name: "IX_VideoStream_MovieId",
                table: "VideoStream",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoStream_VideoRange",
                table: "VideoStream",
                column: "VideoRange");

            migrationBuilder.CreateIndex(
                name: "IX_VideoStream_Width",
                table: "VideoStream",
                column: "Width");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaServerUserConfiguration_MediaServerUsers_UserId",
                table: "MediaServerUserConfiguration",
                column: "UserId",
                principalTable: "MediaServerUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MediaServerUserPolicy_MediaServerUsers_UserId",
                table: "MediaServerUserPolicy",
                column: "UserId",
                principalTable: "MediaServerUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaServerUserConfiguration_MediaServerUsers_UserId",
                table: "MediaServerUserConfiguration");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaServerUserPolicy_MediaServerUsers_UserId",
                table: "MediaServerUserPolicy");

            migrationBuilder.DropTable(
                name: "AudioStream");

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "Filters");

            migrationBuilder.DropTable(
                name: "GenreMovie");

            migrationBuilder.DropTable(
                name: "GenreShow");

            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropTable(
                name: "MediaPerson");

            migrationBuilder.DropTable(
                name: "MediaServerInfo");

            migrationBuilder.DropTable(
                name: "MediaServerStatus");

            migrationBuilder.DropTable(
                name: "MediaSource");

            migrationBuilder.DropTable(
                name: "Plugins");

            migrationBuilder.DropTable(
                name: "RoleClaims");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Statistics");

            migrationBuilder.DropTable(
                name: "SubtitleStream");

            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "UserLogins");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "UserTokens");

            migrationBuilder.DropTable(
                name: "VideoStream");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "People");

            migrationBuilder.DropTable(
                name: "Episodes");

            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.DropTable(
                name: "Season");

            migrationBuilder.DropTable(
                name: "Shows");

            migrationBuilder.DropTable(
                name: "Libraries");

            migrationBuilder.DropTable(
                name: "MediaServerUsers");

            migrationBuilder.DropTable(
                name: "MediaServerUserConfiguration");

            migrationBuilder.DropTable(
                name: "MediaServerUserPolicy");
        }
    }
}
