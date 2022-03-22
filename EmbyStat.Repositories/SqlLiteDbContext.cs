﻿using EmbyStat.Common.SqLite;
using EmbyStat.Common.SqLite.Helpers;
using EmbyStat.Common.SqLite.Movies;
using EmbyStat.Common.SqLite.Shows;
using EmbyStat.Common.SqLite.Streams;
using EmbyStat.Common.SqLite.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SqlPerson = EmbyStat.Common.SqLite.SqlPerson;

namespace EmbyStat.Repositories
{
    public class SqlLiteDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<SqlMovie> Movies { get; set; }
        public DbSet<SqlMediaSource> MediaSources { get; set; }
        public DbSet<SqlVideoStream> VideoStreams { get; set; }
        public DbSet<SqlAudioStream> AudioStreams { get; set; }
        public DbSet<SqlSubtitleStream> SubtitleStreams { get; set; }
        public DbSet<SqlMediaPerson> MediaPerson { get; set; }
        public DbSet<SqlGenre> Genres { get; set; }
        public DbSet<SqlPerson> People { get; set; }
        public DbSet<SqlShow> Shows { get; set; }
        public DbSet<SqlSeason> Seasons { get; set; }
        public DbSet<SqlEpisode> Episodes { get; set; }

        public DbSet<SqlPluginInfo> Plugins { get; set; }
        public DbSet<SqlServerInfo> ServerInfo { get; set; }

        public DbSet<SqlUser> Users { get; set; }
        public DbSet<SqlUserConfiguration> UserConfigurations { get; set; }
        public DbSet<SqlUserPolicy> UserPolicies { get; set; }
        
        public DbSet<SqlDevice> Devices { get; set; }

        public static readonly ILoggerFactory LoggerFactory = 
            Microsoft.Extensions.Logging.LoggerFactory.Create(builder => { builder.AddConsole(); });

        public SqlLiteDbContext()
        {
            
        }

        public SqlLiteDbContext(DbContextOptions<SqlLiteDbContext> options): base(options)
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlite("Data Source=SqliteData.db", x => x.MigrationsAssembly("EmbyStat.Migrations"))
                .UseLoggerFactory(LoggerFactory)  //tie-up DbContext with LoggerFactory object
                .EnableSensitiveDataLogging();
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            BuildPeople(modelBuilder);
            BuildMovies(modelBuilder);
            BuildShows(modelBuilder);
            BuildUsers(modelBuilder);
        }

        private static void BuildUsers(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SqlUser>().HasKey(x => x.Id);
            modelBuilder.Entity<SqlUser>()
                .HasOne<SqlUserConfiguration>()
                .WithOne(x => x.User)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<SqlUser>()
                .HasOne<SqlUserPolicy>()
                .WithOne(x => x.User)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private static void BuildPeople(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SqlMediaPerson>().HasKey(x => x.Id);
            modelBuilder.Entity<SqlMediaPerson>()
                .Property(x => x.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<SqlMediaPerson>().HasIndex(p => p.ShowId);
            modelBuilder.Entity<SqlMediaPerson>().Property(x => x.ShowId).IsRequired(false);
            modelBuilder.Entity<SqlMediaPerson>().HasIndex(p => p.MovieId);
            modelBuilder.Entity<SqlMediaPerson>().Property(x => x.MovieId).IsRequired(false);
            modelBuilder.Entity<SqlMediaPerson>().HasIndex(p => p.PersonId);

            modelBuilder.Entity<SqlShow>()
                .HasMany(x => x.People)
                .WithOne(x => x.Show)
                .IsRequired()
                .HasForeignKey(x => x.ShowId);
            modelBuilder.Entity<SqlMovie>()
                .HasMany(x => x.People)
                .WithOne(x => x.Movie)
                .IsRequired()
                .HasForeignKey(x => x.MovieId);

            modelBuilder.Entity<SqlPerson>()
                .HasMany(x => x.MediaPeople)
                .WithOne(x => x.Person)
                .IsRequired()
                .HasForeignKey(x => x.PersonId);
        }

        private static void BuildShows(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SqlShow>()
                .HasMany(x => x.Seasons)
                .WithOne(x => x.Show)
                .HasForeignKey(x => x.ShowId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SqlGenre>()
                .HasMany(x => x.Shows)
                .WithMany(x => x.Genres);

            modelBuilder.Entity<SqlSeason>()
                .HasMany(x => x.Episodes)
                .WithOne(x => x.Season)
                .HasForeignKey(x => x.SeasonId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SqlEpisode>()
                .HasMany(x => x.SubtitleStreams)
                .WithOne(x => x.Episode)
                .HasForeignKey(x => x.EpisodeId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<SqlEpisode>()
                .HasMany(x => x.MediaSources)
                .WithOne(x => x.Episode)
                .HasForeignKey(x => x.EpisodeId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<SqlEpisode>()
                .HasMany(x => x.AudioStreams)
                .WithOne(x => x.Episode)
                .HasForeignKey(x => x.EpisodeId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<SqlEpisode>()
                .HasMany(x => x.VideoStreams)
                .WithOne(x => x.Episode)
                .HasForeignKey(x => x.EpisodeId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private static void BuildMovies(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SqlMovie>()
                .HasMany(x => x.Genres)
                .WithMany(x => x.Movies);
            modelBuilder.Entity<SqlMovie>()
                .HasMany(x => x.SubtitleStreams)
                .WithOne(x => x.Movie)
                .HasForeignKey(x => x.MovieId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<SqlMovie>()
                .HasMany(x => x.MediaSources)
                .WithOne(x => x.Movie)
                .HasForeignKey(x => x.MovieId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<SqlMovie>()
                .HasMany(x => x.AudioStreams)
                .WithOne(x => x.Movie)
                .HasForeignKey(x => x.MovieId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<SqlMovie>()
                .HasMany(x => x.VideoStreams)
                .WithOne(x => x.Movie)
                .HasForeignKey(x => x.MovieId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
