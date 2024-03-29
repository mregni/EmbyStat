﻿using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Common.Models.Entities.Movies;
using EmbyStat.Common.Models.Entities.Shows;
using EmbyStat.Common.Models.Entities.Streams;
using EmbyStat.Core.DataStore.Seeds;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MediaServerUser = EmbyStat.Common.Models.Entities.Users.MediaServerUser;

namespace EmbyStat.Core.DataStore;

public class EsDbContext : IdentityDbContext<EmbyStatUser>
{
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Person> People { get; set; }
    public DbSet<Show> Shows { get; set; }
    public DbSet<Episode> Episodes { get; set; }

    public DbSet<PluginInfo> Plugins { get; set; }
    public DbSet<MediaServerInfo> MediaServerInfo { get; set; }
    
    public DbSet<MediaServerUser> MediaServerUsers { get; set; }
    public DbSet<Library> Libraries { get; set; }
    public DbSet<LibrarySyncType> LibrarySyncTypes { get; set; }
    public DbSet<Device> Devices { get; set; }
    public DbSet<FilterValues> Filters { get; set; }
    public DbSet<Job> Jobs { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<MediaServerStatus> MediaServerStatus { get; set; }
    public DbSet<Statistic> Statistics { get; set; }
    public DbSet<MediaServerUserView> MediaServerUserViews { get; set; }

    public EsDbContext()
    {
        
    }

    public EsDbContext(DbContextOptions<EsDbContext> options): base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        BuildLibrary(builder);
        BuildPeople(builder);
        BuildMovies(builder);
        BuildShows(builder);
        BuildUsers(builder);
        BuildFilterValues(builder);
        BuildUserViews(builder);
        AddExtraIndexes(builder);

        SeedDatabase(builder);
            
        builder.Entity<IdentityUserLogin<string>>().HasKey(x => new { x.LoginProvider, x.ProviderKey});
        builder.Entity<IdentityUserRole<string>>().HasKey(x => new { x.UserId, x.RoleId});
        builder.Entity<IdentityUserToken<string>>().HasKey(x => new {x.UserId, x.LoginProvider, x.Name});
    }

    #region Builders

    private static void BuildLibrary(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Library>()
            .HasMany(x => x.SyncTypes)
            .WithOne(x => x.Library)
            .HasForeignKey(x => x.LibraryId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
    
    private static void BuildUserViews(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MediaServerUserView>()
            .HasKey(x => new {x.UserId, x.MediaId});
        
        modelBuilder.Entity<MediaServerUserView>()
            .HasOne(x => x.User)
            .WithMany(x => x.Views)
            .HasForeignKey(x => x.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void BuildFilterValues(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FilterValues>().HasKey(x => x.Id);
        modelBuilder.Entity<FilterValues>()
            .Property(x => x._Values)
            .HasColumnName("Values");
    }

    private static void BuildUsers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MediaServerUser>().HasKey(x => x.Id);
    }

    private static void BuildPeople(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MediaPerson>().HasKey(x => x.Id);
        modelBuilder.Entity<MediaPerson>()
            .Property(x => x.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<MediaPerson>().HasIndex(p => p.ShowId);
        modelBuilder.Entity<MediaPerson>().Property(x => x.ShowId).IsRequired(false);
        modelBuilder.Entity<MediaPerson>().HasIndex(p => p.MovieId);
        modelBuilder.Entity<MediaPerson>().Property(x => x.MovieId).IsRequired(false);
        modelBuilder.Entity<MediaPerson>().HasIndex(p => p.PersonId);

        modelBuilder.Entity<Show>()
            .HasMany(x => x.People)
            .WithOne(x => x.Show)
            .IsRequired()
            .HasForeignKey(x => x.ShowId);
        modelBuilder.Entity<Movie>()
            .HasMany(x => x.People)
            .WithOne(x => x.Movie)
            .IsRequired()
            .HasForeignKey(x => x.MovieId);

        modelBuilder.Entity<Person>()
            .HasMany(x => x.MediaPeople)
            .WithOne(x => x.Person)
            .IsRequired()
            .HasForeignKey(x => x.PersonId);
    }

    private static void BuildShows(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Show>().HasIndex(x => x.Primary);
        modelBuilder.Entity<Show>().HasIndex(x => x.Logo);
        modelBuilder.Entity<Show>().HasIndex(x => x.SortName);
        modelBuilder.Entity<Show>().HasIndex(x => x.Name);
        modelBuilder.Entity<Show>().HasIndex(x => x.RunTimeTicks);
        modelBuilder.Entity<Show>().HasIndex(x => x.CommunityRating);
            
        modelBuilder.Entity<Show>()
            .HasMany(x => x.Seasons)
            .WithOne(x => x.Show)
            .HasForeignKey(x => x.ShowId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Show>()
            .HasOne(x => x.Library)
            .WithMany(x => x.Shows)
            .HasForeignKey(x => x.LibraryId);

        modelBuilder.Entity<Genre>()
            .HasMany(x => x.Shows)
            .WithMany(x => x.Genres);

        modelBuilder.Entity<Season>()
            .HasMany(x => x.Episodes)
            .WithOne(x => x.Season)
            .HasForeignKey(x => x.SeasonId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Episode>()
            .HasMany(x => x.SubtitleStreams)
            .WithOne(x => x.Episode)
            .HasForeignKey(x => x.EpisodeId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Episode>()
            .HasMany(x => x.MediaSources)
            .WithOne(x => x.Episode)
            .HasForeignKey(x => x.EpisodeId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Episode>()
            .HasMany(x => x.AudioStreams)
            .WithOne(x => x.Episode)
            .HasForeignKey(x => x.EpisodeId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Episode>()
            .HasMany(x => x.VideoStreams)
            .WithOne(x => x.Episode)
            .HasForeignKey(x => x.EpisodeId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void BuildMovies(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Movie>().HasIndex(x => x.RunTimeTicks);
        modelBuilder.Entity<Movie>().HasIndex(x => x.SortName);
        modelBuilder.Entity<Movie>().HasIndex(x => x.Name);
        modelBuilder.Entity<Movie>().HasIndex(x => x.CommunityRating);
        modelBuilder.Entity<Movie>().HasIndex(x => x.Primary);
        modelBuilder.Entity<Movie>().HasIndex(x => x.Logo);
            
        modelBuilder.Entity<Movie>()
            .HasMany(x => x.Genres)
            .WithMany(x => x.Movies);
        modelBuilder.Entity<Movie>()
            .HasMany(x => x.SubtitleStreams)
            .WithOne(x => x.Movie)
            .HasForeignKey(x => x.MovieId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Movie>()
            .HasMany(x => x.MediaSources)
            .WithOne(x => x.Movie)
            .HasForeignKey(x => x.MovieId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Movie>()
            .HasMany(x => x.AudioStreams)
            .WithOne(x => x.Movie)
            .HasForeignKey(x => x.MovieId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Movie>()
            .HasMany(x => x.VideoStreams)
            .WithOne(x => x.Movie)
            .HasForeignKey(x => x.MovieId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void AddExtraIndexes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VideoStream>().HasIndex(x => x.Width);
        modelBuilder.Entity<VideoStream>().HasIndex(x => x.Height);
        modelBuilder.Entity<VideoStream>().HasIndex(x => x.AverageFrameRate);
        modelBuilder.Entity<VideoStream>().HasIndex(x => x.VideoRange);
        modelBuilder.Entity<VideoStream>().HasIndex(x => x.Codec);
        modelBuilder.Entity<VideoStream>().HasIndex(x => x.BitDepth);
            
        modelBuilder.Entity<MediaSource>().HasIndex(x => x.SizeInMb);
            
        modelBuilder.Entity<SubtitleStream>().HasIndex(x => x.Language);

        modelBuilder.Entity<Genre>().HasIndex(x => x.Name);
    }
    #endregion

    #region Seeders
    private static void SeedDatabase(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Job>().HasData(JobSeed.Jobs);
        modelBuilder.Entity<Language>().HasData(LanguageSeed.Languages);
        modelBuilder.Entity<MediaServerStatus>().HasData(MediaServerSeed.Status);
    }
    #endregion
}