﻿// <auto-generated />
using System;
using EmbyStat.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EmbyStat.Migrations.Sqlite
{
    [DbContext(typeof(SqlLiteDbContext))]
    [Migration("20220306105424_AddServerTables")]
    partial class AddServerTables
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.1");

            modelBuilder.Entity("EmbyStat.Common.SqLite.Helpers.SqlMediaPerson", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("MovieId")
                        .HasColumnType("TEXT");

                    b.Property<string>("PersonId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ShowId")
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("MovieId");

                    b.HasIndex("PersonId");

                    b.HasIndex("ShowId");

                    b.ToTable("MediaPerson");
                });

            modelBuilder.Entity("EmbyStat.Common.SqLite.Movies.SqlMovie", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Banner")
                        .HasColumnType("TEXT");

                    b.Property<string>("CollectionId")
                        .HasColumnType("TEXT");

                    b.Property<decimal?>("CommunityRating")
                        .HasColumnType("TEXT");

                    b.Property<string>("Container")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("DateCreated")
                        .HasColumnType("TEXT");

                    b.Property<string>("IMDB")
                        .HasColumnType("TEXT");

                    b.Property<string>("Logo")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("OfficialRating")
                        .HasColumnType("TEXT");

                    b.Property<string>("OriginalTitle")
                        .HasColumnType("TEXT");

                    b.Property<string>("Path")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("PremiereDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Primary")
                        .HasColumnType("TEXT");

                    b.Property<int?>("ProductionYear")
                        .HasColumnType("INTEGER");

                    b.Property<long?>("RunTimeTicks")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SortName")
                        .HasColumnType("TEXT");

                    b.Property<int?>("TMDB")
                        .HasColumnType("INTEGER");

                    b.Property<string>("TVDB")
                        .HasColumnType("TEXT");

                    b.Property<string>("Thumb")
                        .HasColumnType("TEXT");

                    b.Property<int>("Video3DFormat")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Movies");
                });

            modelBuilder.Entity("EmbyStat.Common.SqLite.Shows.SqlEpisode", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Banner")
                        .HasColumnType("TEXT");

                    b.Property<string>("CollectionId")
                        .HasColumnType("TEXT");

                    b.Property<decimal?>("CommunityRating")
                        .HasColumnType("TEXT");

                    b.Property<string>("Container")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("DateCreated")
                        .HasColumnType("TEXT");

                    b.Property<float?>("DvdEpisodeNumber")
                        .HasColumnType("REAL");

                    b.Property<int?>("DvdSeasonNumber")
                        .HasColumnType("INTEGER");

                    b.Property<string>("IMDB")
                        .HasColumnType("TEXT");

                    b.Property<int?>("IndexNumber")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("IndexNumberEnd")
                        .HasColumnType("INTEGER");

                    b.Property<int>("LocationType")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Logo")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("OfficialRating")
                        .HasColumnType("TEXT");

                    b.Property<string>("Path")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("PremiereDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Primary")
                        .HasColumnType("TEXT");

                    b.Property<int?>("ProductionYear")
                        .HasColumnType("INTEGER");

                    b.Property<long?>("RunTimeTicks")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SeasonId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("SortName")
                        .HasColumnType("TEXT");

                    b.Property<int?>("TMDB")
                        .HasColumnType("INTEGER");

                    b.Property<string>("TVDB")
                        .HasColumnType("TEXT");

                    b.Property<string>("Thumb")
                        .HasColumnType("TEXT");

                    b.Property<int>("Video3DFormat")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("SeasonId");

                    b.ToTable("Episodes");
                });

            modelBuilder.Entity("EmbyStat.Common.SqLite.Shows.SqlSeason", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Banner")
                        .HasColumnType("TEXT");

                    b.Property<string>("CollectionId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("DateCreated")
                        .HasColumnType("TEXT");

                    b.Property<int?>("IndexNumber")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("IndexNumberEnd")
                        .HasColumnType("INTEGER");

                    b.Property<int>("LocationType")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Logo")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Path")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("PremiereDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Primary")
                        .HasColumnType("TEXT");

                    b.Property<int?>("ProductionYear")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ShowId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("SortName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Thumb")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ShowId");

                    b.ToTable("Seasons");
                });

            modelBuilder.Entity("EmbyStat.Common.SqLite.Shows.SqlShow", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Banner")
                        .HasColumnType("TEXT");

                    b.Property<string>("CollectionId")
                        .HasColumnType("TEXT");

                    b.Property<decimal?>("CommunityRating")
                        .HasColumnType("TEXT");

                    b.Property<long?>("CumulativeRunTimeTicks")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("DateCreated")
                        .HasColumnType("TEXT");

                    b.Property<bool>("ExternalSynced")
                        .HasColumnType("INTEGER");

                    b.Property<string>("IMDB")
                        .HasColumnType("TEXT");

                    b.Property<string>("Logo")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("OfficialRating")
                        .HasColumnType("TEXT");

                    b.Property<string>("Path")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("PremiereDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Primary")
                        .HasColumnType("TEXT");

                    b.Property<int?>("ProductionYear")
                        .HasColumnType("INTEGER");

                    b.Property<long?>("RunTimeTicks")
                        .HasColumnType("INTEGER");

                    b.Property<double>("SizeInMb")
                        .HasColumnType("REAL");

                    b.Property<string>("SortName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Status")
                        .HasColumnType("TEXT");

                    b.Property<int?>("TMDB")
                        .HasColumnType("INTEGER");

                    b.Property<string>("TVDB")
                        .HasColumnType("TEXT");

                    b.Property<string>("Thumb")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Shows");
                });

            modelBuilder.Entity("EmbyStat.Common.SqLite.SqlGenre", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Genres");
                });

            modelBuilder.Entity("EmbyStat.Common.SqLite.SqlPerson", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("BirthDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Primary")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("People");
                });

            modelBuilder.Entity("EmbyStat.Common.SqLite.SqlPluginInfo", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("ImageTag")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Version")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Plugins");
                });

            modelBuilder.Entity("EmbyStat.Common.SqLite.SqlServerInfo", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("CachePath")
                        .HasColumnType("TEXT");

                    b.Property<bool>("CanLaunchWebBrowser")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("CanSelfRestart")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("CanSelfUpdate")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("HardwareAccelerationRequiresPremiere")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("HasPendingRestart")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("HasUpdateAvailable")
                        .HasColumnType("INTEGER");

                    b.Property<int>("HttpServerPortNumber")
                        .HasColumnType("INTEGER");

                    b.Property<int>("HttpsPortNumber")
                        .HasColumnType("INTEGER");

                    b.Property<string>("InternalMetadataPath")
                        .HasColumnType("TEXT");

                    b.Property<string>("ItemsByNamePath")
                        .HasColumnType("TEXT");

                    b.Property<string>("LocalAddress")
                        .HasColumnType("TEXT");

                    b.Property<string>("LogPath")
                        .HasColumnType("TEXT");

                    b.Property<string>("OperatingSystem")
                        .HasColumnType("TEXT");

                    b.Property<string>("OperatingSystemDisplayName")
                        .HasColumnType("TEXT");

                    b.Property<string>("ProgramDataPath")
                        .HasColumnType("TEXT");

                    b.Property<string>("ServerName")
                        .HasColumnType("TEXT");

                    b.Property<bool>("SupportsAutoRunAtStartup")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("SupportsHttps")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("SupportsLibraryMonitor")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SystemUpdateLevel")
                        .HasColumnType("TEXT");

                    b.Property<string>("TranscodingTempPath")
                        .HasColumnType("TEXT");

                    b.Property<string>("Version")
                        .HasColumnType("TEXT");

                    b.Property<string>("WanAddress")
                        .HasColumnType("TEXT");

                    b.Property<int>("WebSocketPortNumber")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("ServerInfo");
                });

            modelBuilder.Entity("EmbyStat.Common.SqLite.Streams.SqlAudioStream", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<int?>("BitRate")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ChannelLayout")
                        .HasColumnType("TEXT");

                    b.Property<int?>("Channels")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Codec")
                        .HasColumnType("TEXT");

                    b.Property<string>("EpisodeId")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDefault")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Language")
                        .HasColumnType("TEXT");

                    b.Property<string>("MovieId")
                        .HasColumnType("TEXT");

                    b.Property<int?>("SampleRate")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("EpisodeId");

                    b.HasIndex("MovieId");

                    b.ToTable("AudioStreams");
                });

            modelBuilder.Entity("EmbyStat.Common.SqLite.Streams.SqlMediaSource", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<int?>("BitRate")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Container")
                        .HasColumnType("TEXT");

                    b.Property<string>("EpisodeId")
                        .HasColumnType("TEXT");

                    b.Property<string>("MovieId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Path")
                        .HasColumnType("TEXT");

                    b.Property<string>("Protocol")
                        .HasColumnType("TEXT");

                    b.Property<long?>("RunTimeTicks")
                        .HasColumnType("INTEGER");

                    b.Property<double>("SizeInMb")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.HasIndex("EpisodeId");

                    b.HasIndex("MovieId");

                    b.ToTable("MediaSources");
                });

            modelBuilder.Entity("EmbyStat.Common.SqLite.Streams.SqlSubtitleStream", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Codec")
                        .HasColumnType("TEXT");

                    b.Property<string>("DisplayTitle")
                        .HasColumnType("TEXT");

                    b.Property<string>("EpisodeId")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDefault")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Language")
                        .HasColumnType("TEXT");

                    b.Property<string>("MovieId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("EpisodeId");

                    b.HasIndex("MovieId");

                    b.ToTable("SubtitleStreams");
                });

            modelBuilder.Entity("EmbyStat.Common.SqLite.Streams.SqlVideoStream", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("AspectRatio")
                        .HasColumnType("TEXT");

                    b.Property<float?>("AverageFrameRate")
                        .HasColumnType("REAL");

                    b.Property<int?>("BitDepth")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("BitRate")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("Channels")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Codec")
                        .HasColumnType("TEXT");

                    b.Property<string>("EpisodeId")
                        .HasColumnType("TEXT");

                    b.Property<int?>("Height")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsDefault")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Language")
                        .HasColumnType("TEXT");

                    b.Property<string>("MovieId")
                        .HasColumnType("TEXT");

                    b.Property<string>("VideoRange")
                        .HasColumnType("TEXT");

                    b.Property<int?>("Width")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("EpisodeId");

                    b.HasIndex("MovieId");

                    b.ToTable("VideoStreams");
                });

            modelBuilder.Entity("SqlGenreSqlMovie", b =>
                {
                    b.Property<string>("GenresId")
                        .HasColumnType("TEXT");

                    b.Property<string>("MoviesId")
                        .HasColumnType("TEXT");

                    b.HasKey("GenresId", "MoviesId");

                    b.HasIndex("MoviesId");

                    b.ToTable("SqlGenreSqlMovie");
                });

            modelBuilder.Entity("SqlGenreSqlShow", b =>
                {
                    b.Property<string>("GenresId")
                        .HasColumnType("TEXT");

                    b.Property<string>("ShowsId")
                        .HasColumnType("TEXT");

                    b.HasKey("GenresId", "ShowsId");

                    b.HasIndex("ShowsId");

                    b.ToTable("SqlGenreSqlShow");
                });

            modelBuilder.Entity("EmbyStat.Common.SqLite.Helpers.SqlMediaPerson", b =>
                {
                    b.HasOne("EmbyStat.Common.SqLite.Movies.SqlMovie", "Movie")
                        .WithMany("People")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EmbyStat.Common.SqLite.SqlPerson", "Person")
                        .WithMany("MediaPeople")
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EmbyStat.Common.SqLite.Shows.SqlShow", "Show")
                        .WithMany("People")
                        .HasForeignKey("ShowId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Movie");

                    b.Navigation("Person");

                    b.Navigation("Show");
                });

            modelBuilder.Entity("EmbyStat.Common.SqLite.Shows.SqlEpisode", b =>
                {
                    b.HasOne("EmbyStat.Common.SqLite.Shows.SqlSeason", "Season")
                        .WithMany("Episodes")
                        .HasForeignKey("SeasonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Season");
                });

            modelBuilder.Entity("EmbyStat.Common.SqLite.Shows.SqlSeason", b =>
                {
                    b.HasOne("EmbyStat.Common.SqLite.Shows.SqlShow", "Show")
                        .WithMany("Seasons")
                        .HasForeignKey("ShowId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Show");
                });

            modelBuilder.Entity("EmbyStat.Common.SqLite.Streams.SqlAudioStream", b =>
                {
                    b.HasOne("EmbyStat.Common.SqLite.Shows.SqlEpisode", "Episode")
                        .WithMany("AudioStreams")
                        .HasForeignKey("EpisodeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("EmbyStat.Common.SqLite.Movies.SqlMovie", "Movie")
                        .WithMany("AudioStreams")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Episode");

                    b.Navigation("Movie");
                });

            modelBuilder.Entity("EmbyStat.Common.SqLite.Streams.SqlMediaSource", b =>
                {
                    b.HasOne("EmbyStat.Common.SqLite.Shows.SqlEpisode", "Episode")
                        .WithMany("MediaSources")
                        .HasForeignKey("EpisodeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("EmbyStat.Common.SqLite.Movies.SqlMovie", "Movie")
                        .WithMany("MediaSources")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Episode");

                    b.Navigation("Movie");
                });

            modelBuilder.Entity("EmbyStat.Common.SqLite.Streams.SqlSubtitleStream", b =>
                {
                    b.HasOne("EmbyStat.Common.SqLite.Shows.SqlEpisode", "Episode")
                        .WithMany("SubtitleStreams")
                        .HasForeignKey("EpisodeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("EmbyStat.Common.SqLite.Movies.SqlMovie", "Movie")
                        .WithMany("SubtitleStreams")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Episode");

                    b.Navigation("Movie");
                });

            modelBuilder.Entity("EmbyStat.Common.SqLite.Streams.SqlVideoStream", b =>
                {
                    b.HasOne("EmbyStat.Common.SqLite.Shows.SqlEpisode", "Episode")
                        .WithMany("VideoStreams")
                        .HasForeignKey("EpisodeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("EmbyStat.Common.SqLite.Movies.SqlMovie", "Movie")
                        .WithMany("VideoStreams")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Episode");

                    b.Navigation("Movie");
                });

            modelBuilder.Entity("SqlGenreSqlMovie", b =>
                {
                    b.HasOne("EmbyStat.Common.SqLite.SqlGenre", null)
                        .WithMany()
                        .HasForeignKey("GenresId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EmbyStat.Common.SqLite.Movies.SqlMovie", null)
                        .WithMany()
                        .HasForeignKey("MoviesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SqlGenreSqlShow", b =>
                {
                    b.HasOne("EmbyStat.Common.SqLite.SqlGenre", null)
                        .WithMany()
                        .HasForeignKey("GenresId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EmbyStat.Common.SqLite.Shows.SqlShow", null)
                        .WithMany()
                        .HasForeignKey("ShowsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("EmbyStat.Common.SqLite.Movies.SqlMovie", b =>
                {
                    b.Navigation("AudioStreams");

                    b.Navigation("MediaSources");

                    b.Navigation("People");

                    b.Navigation("SubtitleStreams");

                    b.Navigation("VideoStreams");
                });

            modelBuilder.Entity("EmbyStat.Common.SqLite.Shows.SqlEpisode", b =>
                {
                    b.Navigation("AudioStreams");

                    b.Navigation("MediaSources");

                    b.Navigation("SubtitleStreams");

                    b.Navigation("VideoStreams");
                });

            modelBuilder.Entity("EmbyStat.Common.SqLite.Shows.SqlSeason", b =>
                {
                    b.Navigation("Episodes");
                });

            modelBuilder.Entity("EmbyStat.Common.SqLite.Shows.SqlShow", b =>
                {
                    b.Navigation("People");

                    b.Navigation("Seasons");
                });

            modelBuilder.Entity("EmbyStat.Common.SqLite.SqlPerson", b =>
                {
                    b.Navigation("MediaPeople");
                });
#pragma warning restore 612, 618
        }
    }
}
