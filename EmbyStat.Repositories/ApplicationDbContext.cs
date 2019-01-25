using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Clients.EmbyClient.Model;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Common.Models.Entities.Joins;
using EmbyStat.Common.Models.Tasks.Enum;
using MediaBrowser.Model.Plugins;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Device = EmbyStat.Common.Models.Entities.Device;

namespace EmbyStat.Repositories
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<ConfigurationKeyValue> Configuration { get; set; }
        public DbSet<PluginInfo> Plugins { get; set; }
        public DbSet<ServerInfo> ServerInfo { get; set; }
        public DbSet<Drive> Drives { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Show> Shows { get; set; }
        public DbSet<Season> Seasons { get; set; }
        public DbSet<Episode> Episodes { get; set; }
        public DbSet<Boxset> Boxsets { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<VideoStream> VideoStreams { get; set; }
        public DbSet<AudioStream> AudioStreams { get; set; }
        public DbSet<SubtitleStream> SubtitleStreams { get; set; }
        public DbSet<MediaSource> MediaSources { get; set; }
        public DbSet<ExtraPerson> ExtraPersons { get; set; }
        public DbSet<MediaGenre> MediaGenres { get; set; }
        public DbSet<SeasonEpisode> SeasonEpisodes { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<Statistic> Statistics { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<EmbyStatusKeyValue> EmbyStatus { get; set; }

        public ApplicationDbContext() : base()
        {

        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite("Data Source=data.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ConfigurationKeyValue>().Property(s => s.Id).IsRequired();

            modelBuilder.Entity<PluginInfo>().Property(s => s.Id).IsRequired();

            modelBuilder.Entity<ServerInfo>().Property(s => s.Id).IsRequired();

            modelBuilder.Entity<MediaGenre>().HasKey(mg => mg.Id);
            modelBuilder.Entity<MediaGenre>().HasOne(mg => mg.Media).WithMany(m => m.MediaGenres).HasForeignKey(mg => mg.MediaId);
            modelBuilder.Entity<MediaGenre>().HasOne(mg => mg.Genre).WithMany(g => g.MediaGenres).HasForeignKey(mg => mg.GenreId);

            modelBuilder.Entity<ExtraPerson>().HasKey(ep => ep.Id);
            modelBuilder.Entity<ExtraPerson>().HasOne(ep => ep.Extra).WithMany(e => e.ExtraPersons).HasForeignKey(ep => ep.ExtraId);
            modelBuilder.Entity<ExtraPerson>().HasOne(ep => ep.Person).WithMany(p => p.ExtraPersons).HasForeignKey(ep => ep.PersonId);

            modelBuilder.Entity<SeasonEpisode>().HasKey(ep => ep.Id);
            modelBuilder.Entity<SeasonEpisode>().HasOne(s => s.Episode).WithMany(e => e.SeasonEpisodes).HasForeignKey(s => s.EpisodeId);
            modelBuilder.Entity<SeasonEpisode>().HasOne(s => s.Season).WithMany(s => s.SeasonEpisodes).HasForeignKey(s => s.SeasonId);

            modelBuilder.Entity<Video>().HasMany(v => v.AudioStreams).WithOne(s => s.Video).HasForeignKey(s => s.VideoId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Video>().HasMany(v => v.VideoStreams).WithOne(ms => ms.Video).HasForeignKey(s => s.VideoId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Video>().HasMany(v => v.SubtitleStreams).WithOne(ms => ms.Video).HasForeignKey(s => s.VideoId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Video>().HasMany(v => v.MediaSources).WithOne(ms => ms.Video).HasForeignKey(s => s.VideoId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Movie>().Property(m => m.Id).IsRequired();
            modelBuilder.Entity<Movie>().Property(m => m.ParentId).IsRequired();

            modelBuilder.Entity<Extra>().HasMany(v => v.ExtraPersons).WithOne(s => s.Extra).HasForeignKey(s => s.ExtraId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Media>().HasMany(v => v.MediaGenres).WithOne(s => s.Media).HasForeignKey(s => s.MediaId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Media>().HasMany(v => v.Collections).WithOne(s => s.Media).HasForeignKey(s => s.MediaId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Episode>().Property(m => m.Id).IsRequired();
            modelBuilder.Entity<Episode>().Property(m => m.ParentId).IsRequired(false);
            modelBuilder.Entity<Episode>().HasMany(e => e.SeasonEpisodes).WithOne(s => s.Episode).HasForeignKey(s => s.EpisodeId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Season>().Property(m => m.Id).IsRequired();
            modelBuilder.Entity<Season>().Property(m => m.ParentId).IsRequired();

            modelBuilder.Entity<Show>().Property(m => m.Id).IsRequired();
            modelBuilder.Entity<Show>().Property(m => m.ParentId).IsRequired();

            modelBuilder.Entity<User>().Property(m => m.Id).IsRequired();
            modelBuilder.Entity<User>().Property(m => m.Name).IsRequired().HasMaxLength(100);

            modelBuilder.Entity<MediaSource>().Property(m => m.Id).IsRequired();

            modelBuilder.Entity<AudioStream>().Property(m => m.Id).IsRequired();

            modelBuilder.Entity<VideoStream>().Property(m => m.Id).IsRequired();

            modelBuilder.Entity<SubtitleStream>().Property(m => m.Id).IsRequired();

            modelBuilder.Entity<Boxset>().Property(m => m.Id).IsRequired();
            modelBuilder.Entity<Boxset>().Property(m => m.ParentId).IsRequired();

            modelBuilder.Entity<Collection>().Property(s => s.Id).IsRequired();

            modelBuilder.Entity<Statistic>().Property(s => s.Id).IsRequired();
            modelBuilder.Entity<Statistic>().Property(s => s.CalculationDateTime).IsRequired();
            modelBuilder.Entity<Statistic>().Property(s => s.JsonResult).IsRequired();
            modelBuilder.Entity<Statistic>().HasMany(v => v.Collections).WithOne(s => s.Statistic).HasForeignKey(s => s.StatisticId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Language>().Property(m => m.Id).IsRequired();

            modelBuilder.Entity<EmbyStatusKeyValue>().Property(x => x.Id).IsRequired();

            modelBuilder.Entity<StatisticCollection>().Property(x => x.Id).IsRequired();

            modelBuilder.Entity<MediaCollection>().Property(x => x.Id).IsRequired();

            modelBuilder.Entity<Job>().Property(x => x.Id).IsRequired();

            var listConverter = new ValueConverter<List<string>, string>(
                v => string.Join(";", v),
                v => v.Split(";", StringSplitOptions.RemoveEmptyEntries).ToList());
            var dateTimeOffsetConverter = new ValueConverter<DateTimeOffset, string>(
                v => v.ToString(), 
                v => DateTimeOffset.Parse(v));

            modelBuilder.Entity<User>().HasMany(x => x.AccessSchedules).WithOne().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<User>().Property(x => x.BlockedTags).HasConversion(listConverter);
            modelBuilder.Entity<User>().Property(x => x.BlockUnratedItems).HasConversion(listConverter);
            modelBuilder.Entity<User>().Property(x => x.EnabledDevices).HasConversion(listConverter);
            modelBuilder.Entity<User>().Property(x => x.EnabledChannels).HasConversion(listConverter);
            modelBuilder.Entity<User>().Property(x => x.EnabledFolders).HasConversion(listConverter);
            modelBuilder.Entity<User>().Property(x => x.ExcludedSubFolders).HasConversion(listConverter);
            modelBuilder.Entity<User>().Property(x => x.LastLoginDate).HasConversion(dateTimeOffsetConverter);
            modelBuilder.Entity<User>().Property(x => x.LastActivityDate).HasConversion(dateTimeOffsetConverter);
        }
    }
}
