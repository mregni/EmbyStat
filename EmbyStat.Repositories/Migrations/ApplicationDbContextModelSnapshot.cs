﻿// <auto-generated />

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;

namespace EmbyStat.Repositories.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125");

            modelBuilder.Entity("EmbyStat.Common.Models.Boxset", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("OfficialRating");

                    b.Property<string>("ParentId")
                        .IsRequired();

                    b.Property<string>("Primary");

                    b.HasKey("Id");

                    b.ToTable("Boxsets");
                });

            modelBuilder.Entity("EmbyStat.Common.Models.Configuration", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AccessToken");

                    b.Property<string>("EmbyServerAddress");

                    b.Property<string>("EmbyUserId");

                    b.Property<string>("EmbyUserName");

                    b.Property<string>("Language")
                        .IsRequired();

                    b.Property<string>("ServerName");

                    b.Property<string>("Username");

                    b.Property<bool>("WizardFinished");

                    b.HasKey("Id");

                    b.ToTable("Configuration");
                });

            modelBuilder.Entity("EmbyStat.Common.Models.Device", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AppName");

                    b.Property<string>("AppVersion");

                    b.Property<string>("LastUserName");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Devices");
                });

            modelBuilder.Entity("EmbyStat.Common.Models.Drives", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("Path");

                    b.Property<string>("Type");

                    b.HasKey("Id");

                    b.ToTable("Drives");
                });

            modelBuilder.Entity("EmbyStat.Common.Models.Genre", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Genres");
                });

            modelBuilder.Entity("EmbyStat.Common.Models.Helpers.AudioStream", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long?>("BitRate");

                    b.Property<string>("ChannelLayout");

                    b.Property<int?>("Channels");

                    b.Property<string>("Codec");

                    b.Property<string>("Language");

                    b.Property<int?>("SampleRate");

                    b.Property<string>("VideoId");

                    b.HasKey("Id");

                    b.HasIndex("VideoId");

                    b.ToTable("AudioStreams");
                });

            modelBuilder.Entity("EmbyStat.Common.Models.Helpers.Media", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Banner");

                    b.Property<DateTime?>("DateCreated");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("Logo");

                    b.Property<string>("Name");

                    b.Property<string>("ParentId")
                        .IsRequired();

                    b.Property<string>("Path");

                    b.Property<DateTime?>("PremiereDate");

                    b.Property<string>("Primary");

                    b.Property<int?>("ProductionYear");

                    b.Property<string>("SortName");

                    b.Property<string>("Thumb");

                    b.HasKey("Id");

                    b.ToTable("Media");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Media");
                });

            modelBuilder.Entity("EmbyStat.Common.Models.Helpers.MediaSource", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long?>("BitRate");

                    b.Property<string>("Container");

                    b.Property<string>("Path");

                    b.Property<string>("Protocol");

                    b.Property<long?>("RunTimeTicks");

                    b.Property<string>("VideoId");

                    b.Property<string>("VideoType");

                    b.HasKey("Id");

                    b.HasIndex("VideoId");

                    b.ToTable("MediaSources");
                });

            modelBuilder.Entity("EmbyStat.Common.Models.Helpers.SubtitleStream", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Codec");

                    b.Property<string>("DisplayTitle");

                    b.Property<bool>("IsDefault");

                    b.Property<string>("Language");

                    b.Property<string>("VideoId");

                    b.HasKey("Id");

                    b.HasIndex("VideoId");

                    b.ToTable("SubtitleStreams");
                });

            modelBuilder.Entity("EmbyStat.Common.Models.Helpers.VideoStream", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AspectRatio");

                    b.Property<float?>("AverageFrameRate");

                    b.Property<long?>("BitRate");

                    b.Property<int?>("Channels");

                    b.Property<int?>("Height");

                    b.Property<string>("Language");

                    b.Property<string>("VideoId");

                    b.Property<int?>("Width");

                    b.HasKey("Id");

                    b.HasIndex("VideoId");

                    b.ToTable("VideoStreams");
                });

            modelBuilder.Entity("EmbyStat.Common.Models.Joins.ExtraPerson", b =>
                {
                    b.Property<string>("ExtraId");

                    b.Property<string>("PersonId");

                    b.HasKey("ExtraId", "PersonId");

                    b.HasIndex("PersonId");

                    b.ToTable("ExtraPersons");
                });

            modelBuilder.Entity("EmbyStat.Common.Models.Joins.MediaGenre", b =>
                {
                    b.Property<string>("GenreId");

                    b.Property<string>("MediaId");

                    b.HasKey("GenreId", "MediaId");

                    b.HasIndex("MediaId");

                    b.ToTable("MediaGenres");
                });

            modelBuilder.Entity("EmbyStat.Common.Models.Person", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("Type");

                    b.HasKey("Id");

                    b.ToTable("People");
                });

            modelBuilder.Entity("EmbyStat.Common.Models.Server", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("CanSelfRestart");

                    b.Property<bool>("CanSelfUpdate");

                    b.Property<bool>("HasPendingRestart");

                    b.Property<bool>("HasUpdateAvailable");

                    b.Property<int>("HttpServerPortNumber");

                    b.Property<int>("HttpsPortNumber");

                    b.Property<string>("LocalAddress");

                    b.Property<string>("MacAddress");

                    b.Property<string>("OperatingSystem");

                    b.Property<string>("OperatingSystemDispayName");

                    b.Property<string>("ServerName");

                    b.Property<string>("SystemArchitecture");

                    b.Property<string>("SystemUpdateLevel");

                    b.Property<string>("Version");

                    b.Property<string>("WanAddress");

                    b.Property<int>("WebSocketPortNumber");

                    b.HasKey("Id");

                    b.ToTable("Servers");
                });

            modelBuilder.Entity("EmbyStat.Common.Models.ServerInfo", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CachePath");

                    b.Property<bool>("CanLaunchWebBrowser");

                    b.Property<bool>("CanSelfRestart");

                    b.Property<bool>("CanSelfUpdate");

                    b.Property<string>("EncoderLocationType");

                    b.Property<bool>("HasPendingRestart");

                    b.Property<bool>("HasUpdateAvailable");

                    b.Property<int>("HttpServerPortNumber");

                    b.Property<int>("HttpsPortNumber");

                    b.Property<string>("InternalMetadataPath");

                    b.Property<bool>("IsShuttingDown");

                    b.Property<string>("ItemsByNamePath");

                    b.Property<string>("LogPath");

                    b.Property<string>("OperatingSystemDisplayName");

                    b.Property<string>("PackageName");

                    b.Property<string>("ProgramDataPath");

                    b.Property<bool>("SupportsAutoRunAtStartup");

                    b.Property<bool>("SupportsHttps");

                    b.Property<bool>("SupportsLibraryMonitor");

                    b.Property<int>("SystemArchitecture");

                    b.Property<int>("SystemUpdateLevel");

                    b.Property<string>("TranscodingTempPath");

                    b.Property<int>("WebSocketPortNumber");

                    b.HasKey("Id");

                    b.ToTable("ServerInfo");
                });

            modelBuilder.Entity("EmbyStat.Common.Models.User", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsAdmin");

                    b.Property<bool>("IsDisabled");

                    b.Property<bool>("IsHidden");

                    b.Property<DateTime?>("LastActivityDate");

                    b.Property<DateTime?>("LastLoginDate");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("ServerId");

                    b.HasKey("Id");

                    b.HasIndex("ServerId");

                    b.ToTable("User");
                });

            modelBuilder.Entity("EmbyStat.Common.Tasks.TaskResult", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("EndTimeUtc");

                    b.Property<string>("ErrorMessage");

                    b.Property<string>("Key");

                    b.Property<string>("LongErrorMessage");

                    b.Property<string>("Name");

                    b.Property<DateTime>("StartTimeUtc");

                    b.Property<int>("Status");

                    b.HasKey("Id");

                    b.ToTable("TaskResults");
                });

            modelBuilder.Entity("EmbyStat.Common.Tasks.TaskTriggerInfo", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("DayOfWeek");

                    b.Property<long?>("IntervalTicks");

                    b.Property<long?>("MaxRuntimeTicks");

                    b.Property<string>("TaskKey");

                    b.Property<long?>("TimeOfDayTicks");

                    b.Property<string>("Type");

                    b.HasKey("Id");

                    b.ToTable("TaskTriggerInfos");
                });

            modelBuilder.Entity("MediaBrowser.Model.Plugins.PluginInfo", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AssemblyFileName");

                    b.Property<DateTime>("ConfigurationDateLastModified");

                    b.Property<string>("ConfigurationFileName");

                    b.Property<string>("Description");

                    b.Property<string>("ImageUrl");

                    b.Property<string>("Name");

                    b.Property<string>("Version");

                    b.HasKey("Id");

                    b.ToTable("Plugins");
                });

            modelBuilder.Entity("EmbyStat.Common.Models.Helpers.Extra", b =>
                {
                    b.HasBaseType("EmbyStat.Common.Models.Helpers.Media");

                    b.Property<float?>("CommunityRating");

                    b.Property<string>("IMDB");

                    b.Property<string>("Overview");

                    b.Property<long?>("RunTimeTicks");

                    b.Property<string>("TMDB");

                    b.Property<string>("TVDB");

                    b.ToTable("Extra");

                    b.HasDiscriminator().HasValue("Extra");
                });

            modelBuilder.Entity("EmbyStat.Common.Models.Season", b =>
                {
                    b.HasBaseType("EmbyStat.Common.Models.Helpers.Media");

                    b.Property<int?>("IndexNumber")
                        .HasColumnName("Season_IndexNumber");

                    b.Property<int?>("IndexNumberEnd")
                        .HasColumnName("Season_IndexNumberEnd");

                    b.ToTable("Season");

                    b.HasDiscriminator().HasValue("Season");
                });

            modelBuilder.Entity("EmbyStat.Common.Models.Helpers.Video", b =>
                {
                    b.HasBaseType("EmbyStat.Common.Models.Helpers.Extra");

                    b.Property<string>("Container");

                    b.Property<bool?>("HasSubtitles");

                    b.Property<bool?>("IdHD");

                    b.Property<string>("MediaType");

                    b.ToTable("Video");

                    b.HasDiscriminator().HasValue("Video");
                });

            modelBuilder.Entity("EmbyStat.Common.Models.Show", b =>
                {
                    b.HasBaseType("EmbyStat.Common.Models.Helpers.Extra");

                    b.Property<long?>("CumulativeRunTimeTicks");

                    b.Property<DateTime?>("DateLastMediaAdded");

                    b.Property<string>("HomePageUrl")
                        .HasColumnName("Show_HomePageUrl");

                    b.Property<string>("OfficialRating")
                        .HasColumnName("Show_OfficialRating");

                    b.Property<string>("Status");

                    b.ToTable("Show");

                    b.HasDiscriminator().HasValue("Show");
                });

            modelBuilder.Entity("EmbyStat.Common.Models.Episode", b =>
                {
                    b.HasBaseType("EmbyStat.Common.Models.Helpers.Video");

                    b.Property<float?>("DvdEpisodeNumber");

                    b.Property<int?>("DvdSeasonNumber");

                    b.Property<int?>("IndexNumber");

                    b.Property<int?>("IndexNumberEnd");

                    b.ToTable("Episode");

                    b.HasDiscriminator().HasValue("Episode");
                });

            modelBuilder.Entity("EmbyStat.Common.Models.Movie", b =>
                {
                    b.HasBaseType("EmbyStat.Common.Models.Helpers.Video");

                    b.Property<string>("HomePageUrl");

                    b.Property<string>("OfficialRating");

                    b.Property<string>("OriginalTitle");

                    b.ToTable("Movie");

                    b.HasDiscriminator().HasValue("Movie");
                });

            modelBuilder.Entity("EmbyStat.Common.Models.Helpers.AudioStream", b =>
                {
                    b.HasOne("EmbyStat.Common.Models.Helpers.Video", "Video")
                        .WithMany("AudioStreams")
                        .HasForeignKey("VideoId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("EmbyStat.Common.Models.Helpers.MediaSource", b =>
                {
                    b.HasOne("EmbyStat.Common.Models.Helpers.Video", "Video")
                        .WithMany("MediaSources")
                        .HasForeignKey("VideoId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("EmbyStat.Common.Models.Helpers.SubtitleStream", b =>
                {
                    b.HasOne("EmbyStat.Common.Models.Helpers.Video", "Video")
                        .WithMany("SubtitleStreams")
                        .HasForeignKey("VideoId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("EmbyStat.Common.Models.Helpers.VideoStream", b =>
                {
                    b.HasOne("EmbyStat.Common.Models.Helpers.Video", "Video")
                        .WithMany("VideoStreams")
                        .HasForeignKey("VideoId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("EmbyStat.Common.Models.Joins.ExtraPerson", b =>
                {
                    b.HasOne("EmbyStat.Common.Models.Helpers.Extra", "Extra")
                        .WithMany("ExtraPersons")
                        .HasForeignKey("ExtraId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("EmbyStat.Common.Models.Person", "Person")
                        .WithMany("ExtraPersons")
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("EmbyStat.Common.Models.Joins.MediaGenre", b =>
                {
                    b.HasOne("EmbyStat.Common.Models.Genre", "Genre")
                        .WithMany("MediaGenres")
                        .HasForeignKey("GenreId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("EmbyStat.Common.Models.Helpers.Media", "Media")
                        .WithMany("MediaGenres")
                        .HasForeignKey("MediaId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("EmbyStat.Common.Models.User", b =>
                {
                    b.HasOne("EmbyStat.Common.Models.Server", "Server")
                        .WithMany("Users")
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
