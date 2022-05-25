using System.IdentityModel.Tokens.Jwt;
using Aiursoft.XelNaga.Services;
using EmbyStat.Clients.Base;
using EmbyStat.Clients.Base.Api;
using EmbyStat.Clients.Base.Http;
using EmbyStat.Clients.Base.WebSocket;
using EmbyStat.Clients.Emby;
using EmbyStat.Clients.Emby.Http;
using EmbyStat.Clients.Emby.WebSocket;
using EmbyStat.Clients.GitHub;
using EmbyStat.Clients.Jellyfin;
using EmbyStat.Clients.Jellyfin.Http;
using EmbyStat.Clients.Tmdb;
using EmbyStat.Common.Exceptions;
using EmbyStat.Configuration;
using EmbyStat.Configuration.Interfaces;
using EmbyStat.Core.About;
using EmbyStat.Core.Account;
using EmbyStat.Core.DataStore;
using EmbyStat.Core.Filters;
using EmbyStat.Core.Filters.Interfaces;
using EmbyStat.Core.Genres;
using EmbyStat.Core.Hubs;
using EmbyStat.Core.Jobs;
using EmbyStat.Core.Jobs.Interfaces;
using EmbyStat.Core.Languages;
using EmbyStat.Core.Languages.Interfaces;
using EmbyStat.Core.Logs;
using EmbyStat.Core.Logs.Interfaces;
using EmbyStat.Core.MediaServers;
using EmbyStat.Core.MediaServers.Interfaces;
using EmbyStat.Core.Movies;
using EmbyStat.Core.Movies.Interfaces;
using EmbyStat.Core.People;
using EmbyStat.Core.People.Interfaces;
using EmbyStat.Core.Sessions;
using EmbyStat.Core.Sessions.Interfaces;
using EmbyStat.Core.Shows;
using EmbyStat.Core.Shows.Interfaces;
using EmbyStat.Core.Statistics;
using EmbyStat.Core.Statistics.Interfaces;
using EmbyStat.Core.System;
using EmbyStat.Core.System.Interfaces;
using EmbyStat.Core.Updates;
using EmbyStat.Core.Updates.Interfaces;
using EmbyStat.Jobs;
using EmbyStat.Jobs.Jobs.Interfaces;
using EmbyStat.Jobs.Jobs.Maintenance;
using EmbyStat.Jobs.Jobs.Sync;
using EmbyStat.Jobs.Jobs.Updater;
using EmbyStat.Repositories.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EmbyStat.DI;

public static class DiExtensions
{
    public static void RegisterApplicationDependencies(this IServiceCollection services)
    {
        services.RegisterServices();
        services.RegisterRepositories();
        services.RegisterJobs();
        services.RegisterClients();
        services.RegisterHttp();
        services.RegisterSignalR();
        services.RegisterUserHandlers();
    }

    private static void RegisterUserHandlers(this IServiceCollection services)
    {
        services.TryAddSingleton<JwtSecurityTokenHandler>();
    }

    private static void RegisterServices(this IServiceCollection services)
    {
        services.TryAddTransient<IAboutService, AboutService>();
        services.TryAddTransient<IAccountService, AccountService>();
        services.TryAddTransient<IMediaServerService, MediaServerService>();
        services.TryAddTransient<IJobService, JobService>();
        services.TryAddTransient<IFilterService, FilterService>();
        services.TryAddTransient<ILanguageService, LanguageService>();
        services.TryAddTransient<ILogService, LogService>();
        services.TryAddTransient<IMovieService, MovieService>();
        services.TryAddTransient<ISessionService, SessionService>();
        services.TryAddSingleton<IConfigurationService, ConfigurationService>();
        services.TryAddTransient<IShowService, ShowService>();
        services.TryAddTransient<ISystemService, SystemService>();
        services.TryAddTransient<IUpdateService, UpdateService>();

        services.TryAddTransient<CannonService>();
    }

    private static void RegisterRepositories(this IServiceCollection services)
    {
        services.TryAddTransient<ISqliteBootstrap, SqliteBootstrap>();

        services.TryAddTransient<IMovieRepository, MovieRepository>();
        services.TryAddTransient<IMediaServerRepository, MediaServerRepository>();
        services.TryAddTransient<IPersonRepository, PersonRepository>();
        services.TryAddTransient<IFilterRepository, FilterRepository>();
        services.TryAddTransient<IShowRepository, ShowRepository>();
        services.TryAddTransient<IStatisticsRepository, StatisticsRepository>();
        services.TryAddTransient<ILanguageRepository, LanguageRepository>();
        services.TryAddTransient<IJobRepository, JobRepository>();
        services.TryAddTransient<IGenreRepository, GenreRepository>();
    }

    private static void RegisterJobs(this IServiceCollection services)
    {
        services.TryAddSingleton<IBackgroundJobClient, BackgroundJobClient>();
        services.TryAddTransient<IJobInitializer, JobInitializer>();
        services.TryAddSingleton<IRecurringJobManager, RecurringJobManager>();

        services.TryAddTransient<IDatabaseCleanupJob, DatabaseCleanupJob>();
        services.TryAddTransient<IPingEmbyJob, PingEmbyJob>();
        services.TryAddTransient<IShowSyncJob, ShowSyncJob>();
        services.TryAddTransient<IMovieSyncJob, MovieSyncJob>();
        services.TryAddTransient<ISmallSyncJob, SmallSyncJob>();
        services.TryAddTransient<ICheckUpdateJob, CheckUpdateJob>();
    }

    private static void RegisterClients(this IServiceCollection services)
    {
        services.AddSingleton<IClientStrategy, ClientStrategy>();
        services.AddSingleton<IClientFactory, EmbyClientFactory>();
        services.AddSingleton<IClientFactory, JellyfinClientFactory>();
            
        services.TryAddSingleton<IEmbyBaseHttpClient, EmbyBaseHttpClient>();
        services.TryAddSingleton<IJellyfinBaseHttpClient, JellyfinBaseHttpClient>();

        services.TryAddTransient<ITmdbClient, TmdbClient>();
        services.TryAddTransient<IGitHubClient, GitHubClient>();

        services.TryAddSingleton<IWebSocketApi, WebSocketApi>();
        services.TryAddSingleton<IWebSocketClient, EmbyWebSocketClient>();
    }

    private static void RegisterHttp(this IServiceCollection services)
    {
        services.TryAddTransient<BusinessExceptionFilterAttribute>();
        services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

        services.TryAddTransient<IRefitHttpClientFactory<IMediaServerApi>, RefitHttpClientFactory<IMediaServerApi>>();
        services.TryAddTransient<IRefitHttpClientFactory<IGitHubApi>, RefitHttpClientFactory<IGitHubApi>>();
    }

    private static void RegisterSignalR(this IServiceCollection services)
    {
        services.TryAddSingleton<IHubHelper, HubHelper>();
    }
}