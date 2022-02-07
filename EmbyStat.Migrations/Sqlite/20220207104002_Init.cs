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
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    OriginalTitle = table.Column<string>(type: "TEXT", nullable: true),
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
                    CollectionId = table.Column<string>(type: "TEXT", nullable: true),
                    CommunityRating = table.Column<float>(type: "REAL", nullable: true),
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
                });

            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    MovieCount = table.Column<int>(type: "INTEGER", nullable: true),
                    BirthDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ShowCount = table.Column<int>(type: "INTEGER", nullable: true),
                    Primary = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.Id);
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
                    CollectionId = table.Column<string>(type: "TEXT", nullable: true),
                    CommunityRating = table.Column<float>(type: "REAL", nullable: true),
                    IMDB = table.Column<string>(type: "TEXT", nullable: true),
                    TMDB = table.Column<int>(type: "INTEGER", nullable: true),
                    TVDB = table.Column<string>(type: "TEXT", nullable: true),
                    RunTimeTicks = table.Column<long>(type: "INTEGER", nullable: true),
                    OfficialRating = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shows", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Seasons",
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
                    SortName = table.Column<string>(type: "TEXT", nullable: true),
                    CollectionId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seasons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Seasons_Shows_ShowId",
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
                    CollectionId = table.Column<string>(type: "TEXT", nullable: true),
                    CommunityRating = table.Column<float>(type: "REAL", nullable: true),
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
                        name: "FK_Episodes_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AudioStreams",
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
                    table.PrimaryKey("PK_AudioStreams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AudioStreams_Episodes_EpisodeId",
                        column: x => x.EpisodeId,
                        principalTable: "Episodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AudioStreams_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    SqlEpisodeId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Genres_Episodes_SqlEpisodeId",
                        column: x => x.SqlEpisodeId,
                        principalTable: "Episodes",
                        principalColumn: "Id");
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
                    EpisodeId = table.Column<string>(type: "TEXT", nullable: true),
                    PersonId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaPerson", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaPerson_Episodes_EpisodeId",
                        column: x => x.EpisodeId,
                        principalTable: "Episodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "MediaSources",
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
                    table.PrimaryKey("PK_MediaSources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaSources_Episodes_EpisodeId",
                        column: x => x.EpisodeId,
                        principalTable: "Episodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MediaSources_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubtitleStreams",
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
                    table.PrimaryKey("PK_SubtitleStreams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubtitleStreams_Episodes_EpisodeId",
                        column: x => x.EpisodeId,
                        principalTable: "Episodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubtitleStreams_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VideoStreams",
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
                    table.PrimaryKey("PK_VideoStreams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideoStreams_Episodes_EpisodeId",
                        column: x => x.EpisodeId,
                        principalTable: "Episodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VideoStreams_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SqlGenreSqlMovie",
                columns: table => new
                {
                    GenresId = table.Column<string>(type: "TEXT", nullable: false),
                    MoviesId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SqlGenreSqlMovie", x => new { x.GenresId, x.MoviesId });
                    table.ForeignKey(
                        name: "FK_SqlGenreSqlMovie_Genres_GenresId",
                        column: x => x.GenresId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SqlGenreSqlMovie_Movies_MoviesId",
                        column: x => x.MoviesId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SqlGenreSqlShow",
                columns: table => new
                {
                    GenresId = table.Column<string>(type: "TEXT", nullable: false),
                    ShowsId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SqlGenreSqlShow", x => new { x.GenresId, x.ShowsId });
                    table.ForeignKey(
                        name: "FK_SqlGenreSqlShow_Genres_GenresId",
                        column: x => x.GenresId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SqlGenreSqlShow_Shows_ShowsId",
                        column: x => x.ShowsId,
                        principalTable: "Shows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AudioStreams_EpisodeId",
                table: "AudioStreams",
                column: "EpisodeId");

            migrationBuilder.CreateIndex(
                name: "IX_AudioStreams_MovieId",
                table: "AudioStreams",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_Episodes_SeasonId",
                table: "Episodes",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_Genres_SqlEpisodeId",
                table: "Genres",
                column: "SqlEpisodeId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaPerson_EpisodeId",
                table: "MediaPerson",
                column: "EpisodeId");

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
                name: "IX_MediaSources_EpisodeId",
                table: "MediaSources",
                column: "EpisodeId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaSources_MovieId",
                table: "MediaSources",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_Seasons_ShowId",
                table: "Seasons",
                column: "ShowId");

            migrationBuilder.CreateIndex(
                name: "IX_SqlGenreSqlMovie_MoviesId",
                table: "SqlGenreSqlMovie",
                column: "MoviesId");

            migrationBuilder.CreateIndex(
                name: "IX_SqlGenreSqlShow_ShowsId",
                table: "SqlGenreSqlShow",
                column: "ShowsId");

            migrationBuilder.CreateIndex(
                name: "IX_SubtitleStreams_EpisodeId",
                table: "SubtitleStreams",
                column: "EpisodeId");

            migrationBuilder.CreateIndex(
                name: "IX_SubtitleStreams_MovieId",
                table: "SubtitleStreams",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoStreams_EpisodeId",
                table: "VideoStreams",
                column: "EpisodeId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoStreams_MovieId",
                table: "VideoStreams",
                column: "MovieId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AudioStreams");

            migrationBuilder.DropTable(
                name: "MediaPerson");

            migrationBuilder.DropTable(
                name: "MediaSources");

            migrationBuilder.DropTable(
                name: "SqlGenreSqlMovie");

            migrationBuilder.DropTable(
                name: "SqlGenreSqlShow");

            migrationBuilder.DropTable(
                name: "SubtitleStreams");

            migrationBuilder.DropTable(
                name: "VideoStreams");

            migrationBuilder.DropTable(
                name: "People");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.DropTable(
                name: "Episodes");

            migrationBuilder.DropTable(
                name: "Seasons");

            migrationBuilder.DropTable(
                name: "Shows");
        }
    }
}
