﻿using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Helpers;
using EmbyStat.Common.Models.Joins;
using EmbyStat.Common.Tasks;
using MediaBrowser.Model.Plugins;
using Microsoft.EntityFrameworkCore;

namespace EmbyStat.Repositories
{
    public class ApplicationDbContext : DbContext
    {
	    public DbSet<Configuration> Configuration { get; set; }
		public DbSet<PluginInfo> Plugins { get; set; }
		public DbSet<ServerInfo> ServerInfo { get; set; }
		public DbSet<Drives> Drives { get; set; }
        public DbSet<TaskResult> TaskResults { get; set; }
        public DbSet<TaskTriggerInfo> TaskTriggerInfos { get; set; }
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
        public DbSet<Server> Servers { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<Statistic> Statistics { get; set; }
        public DbSet<Language> Languages { get; set; }

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

		    modelBuilder.Entity<Configuration>().Property(s => s.Id).IsRequired();
		    modelBuilder.Entity<Configuration>().Property(s => s.Language).IsRequired();

		    modelBuilder.Entity<PluginInfo>().Property(s => s.Id).IsRequired();

		    modelBuilder.Entity<ServerInfo>().Property(s => s.Id).IsRequired();

	        modelBuilder.Entity<TaskTriggerInfo>().Property(t => t.Id).IsRequired();

            modelBuilder.Entity<MediaGenre>().HasKey(mg => new { mg.GenreId, mg.MediaId });
            modelBuilder.Entity<MediaGenre>().HasOne(mg => mg.Media).WithMany(m => m.MediaGenres).HasForeignKey(mg => mg.MediaId);
            modelBuilder.Entity<MediaGenre>().HasOne(mg => mg.Genre).WithMany(g => g.MediaGenres).HasForeignKey(mg => mg.GenreId);

            modelBuilder.Entity<ExtraPerson>().HasKey(ep => new { ep.ExtraId, ep.PersonId });
            modelBuilder.Entity<ExtraPerson>().HasOne(ep => ep.Extra).WithMany(e => e.ExtraPersons).HasForeignKey(ep => ep.ExtraId);
            modelBuilder.Entity<ExtraPerson>().HasOne(ep => ep.Person).WithMany(p => p.ExtraPersons).HasForeignKey(ep => ep.PersonId);

	        modelBuilder.Entity<SeasonEpisode>().HasKey(s => new {s.EpisodeId, s.SeasonId});
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

            modelBuilder.Entity<Episode>().Property(m => m.Id).IsRequired();
            modelBuilder.Entity<Episode>().Property(m => m.ParentId).IsRequired(false);
            modelBuilder.Entity<Episode>().HasMany(e => e.SeasonEpisodes).WithOne(s => s.Episode).HasForeignKey(s => s.EpisodeId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Season>().Property(m => m.Id).IsRequired();
            modelBuilder.Entity<Season>().Property(m => m.ParentId).IsRequired();

            modelBuilder.Entity<Show>().Property(m => m.Id).IsRequired();
            modelBuilder.Entity<Show>().Property(m => m.ParentId).IsRequired();

            modelBuilder.Entity<User>().Property(m => m.Id).IsRequired();
            modelBuilder.Entity<User>().Property(m => m.Name).IsRequired().HasMaxLength(100);

            modelBuilder.Entity<Server>().Property(m => m.Id).IsRequired();
            modelBuilder.Entity<Server>().HasMany(s => s.Users).WithOne(u => u.Server).HasForeignKey(u => u.ServerId).OnDelete(DeleteBehavior.Cascade);

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

	        modelBuilder.Entity<Language>().Property(m => m.Id).IsRequired();
        }
    }
}
