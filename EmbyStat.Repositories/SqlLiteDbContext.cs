using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Common.SqLite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
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
        public DbSet<SqlMoviePerson> SqlMovieSqlPerson { get; set; }
        public DbSet<SqlGenre> Genres { get; set; }
        public DbSet<SqlPerson> People { get; set; }

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
            optionsBuilder.UseLoggerFactory(LoggerFactory)  //tie-up DbContext with LoggerFactory object
                .EnableSensitiveDataLogging();
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SqlMoviePerson>().HasKey(p => new { p.MovieId, p.PersonId, p.Type });
            modelBuilder.Entity<SqlMovie>()
                .HasMany(x => x.MoviePeople)
                .WithOne(x => x.Movie)
                .IsRequired()
                .HasForeignKey(x => x.MovieId);

            modelBuilder.Entity<SqlPerson>()
                .HasMany(x => x.MoviePeople)
                .WithOne(x => x.Person)
                .IsRequired()
                .HasForeignKey(x => x.PersonId);

            modelBuilder.Entity<SqlGenre>()
                .HasMany(x => x.Movies)
                .WithMany(x => x.Genres);

            modelBuilder.Entity<SqlMovie>()
                .HasMany(x => x.Genres)
                .WithMany(x => x.Movies);
            modelBuilder.Entity<SqlMovie>()
                .HasMany(x => x.SubtitleStreams)
                .WithOne(x => x.Movie)
                .HasForeignKey(x => x.MovieId)
                .OnDelete(DeleteBehavior.ClientCascade);
            modelBuilder.Entity<SqlMovie>()
                .HasMany(x => x.MediaSources)
                .WithOne(x => x.Movie)
                .HasForeignKey(x => x.MovieId)
                .OnDelete(DeleteBehavior.ClientCascade);
            modelBuilder.Entity<SqlMovie>()
                .HasMany(x => x.AudioStreams)
                .WithOne(x => x.Movie)
                .HasForeignKey(x => x.MovieId)
                .OnDelete(DeleteBehavior.ClientCascade);
            modelBuilder.Entity<SqlMovie>()
                .HasMany(x => x.VideoStreams)
                .WithOne(x => x.Movie)
                .HasForeignKey(x => x.MovieId)
                .OnDelete(DeleteBehavior.ClientCascade);
        }
    }
}
