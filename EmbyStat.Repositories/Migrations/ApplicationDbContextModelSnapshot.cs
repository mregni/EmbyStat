﻿// <auto-generated />
using EmbyStat.Repositories;
using MediaBrowser.Model.System;
using MediaBrowser.Model.Tasks;
using MediaBrowser.Model.Updates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
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

            modelBuilder.Entity("EmbyStat.Repositories.Config.Configuration", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AccessToken");

                    b.Property<string>("EmbyServerAddress");

                    b.Property<string>("EmbyUserName");

                    b.Property<string>("Language")
                        .IsRequired();

                    b.Property<string>("ServerName");

                    b.Property<string>("UserId");

                    b.Property<string>("Username");

                    b.Property<bool>("WizardFinished");

                    b.HasKey("Id");

                    b.ToTable("Configuration");
                });

            modelBuilder.Entity("EmbyStat.Repositories.EmbyDrive.Drives", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("Path");

                    b.Property<string>("Type");

                    b.HasKey("Id");

                    b.ToTable("Drives");
                });

            modelBuilder.Entity("EmbyStat.Repositories.EmbyHeartBeat.Ping", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Success");

                    b.Property<DateTime>("Time");

                    b.HasKey("Id");

                    b.ToTable("Pings");
                });

            modelBuilder.Entity("EmbyStat.Repositories.EmbyServerInfo.ServerInfo", b =>
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

            modelBuilder.Entity("EmbyStat.Repositories.EmbyTask.TaskTriggerInfo", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("DayOfWeek");

                    b.Property<long?>("IntervalTicks");

                    b.Property<long?>("MaxRuntimeTicks");

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

            modelBuilder.Entity("MediaBrowser.Model.Tasks.TaskResult", b =>
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
#pragma warning restore 612, 618
        }
    }
}
