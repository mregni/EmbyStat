using System.IdentityModel.Tokens.Jwt;
using Aiursoft.XelNaga.Services;
using EmbyStat.Clients.Base;
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
using EmbyStat.Common.Hubs;
using EmbyStat.Jobs;
using EmbyStat.Jobs.Jobs.Interfaces;
using EmbyStat.Jobs.Jobs.Maintenance;
using EmbyStat.Jobs.Jobs.Sync;
using EmbyStat.Jobs.Jobs.Updater;
using EmbyStat.Repositories;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services;
using EmbyStat.Services.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RestSharp;

namespace EmbyStat.DI
{
    public static class DIExtensions
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
            services.TryAddSingleton<ISettingsService, SettingsService>();
            services.TryAddTransient<IShowService, ShowService>();
            services.TryAddTransient<ISystemService, SystemService>();
            services.TryAddTransient<IUpdateService, UpdateService>();

            services.TryAddTransient<CannonService>();
        }

        private static void RegisterRepositories(this IServiceCollection services)
        {
            services.TryAddTransient<ISqliteBootstrap, SqliteBootstrap>();

            services.TryAddTransient<IMovieRepository, SqlMovieRepository>();
            services.TryAddTransient<IMediaServerRepository, SqlMediaServerRepository>();
            services.TryAddTransient<IPersonRepository, SqlPersonRepository>();
            services.TryAddTransient<IFilterRepository, SqlFilterRepository>();
            services.TryAddTransient<IShowRepository, SqlShowRepository>();
            services.TryAddTransient<IStatisticsRepository, SqlStatisticsRepository>();
            services.TryAddTransient<ILanguageRepository, LanguageRepository>();
            services.TryAddTransient<IJobRepository, SqlJobRepository>();
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
            services.AddTransient<IRestClient, RestClient>();

            services.AddSingleton<IClientStrategy, ClientStrategy>();
            services.AddSingleton<IClientFactory, EmbyClientFactory>();
            services.AddSingleton<IClientFactory, JellyfinClientFactory>();
            
            services.TryAddSingleton<IEmbyBaseHttpClient, EmbyBaseHttpClient>();
            services.TryAddSingleton<IJellyfinBaseHttpClient, JellyfinBaseHttpClient>();

            services.TryAddTransient<ITmdbClient, TmdbClient>();
            services.TryAddTransient<IGithubClient, GithubClient>();

            services.TryAddSingleton<IWebSocketApi, WebSocketApi>();
            services.TryAddSingleton<IWebSocketClient, EmbyWebSocketClient>();
        }

        private static void RegisterHttp(this IServiceCollection services)
        {
            services.TryAddTransient<BusinessExceptionFilterAttribute>();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.TryAddTransient<IRefitHttpClientFactory<INewBaseClient>, RefitHttpClientFactory<INewBaseClient>>();
        }

        private static void RegisterSignalR(this IServiceCollection services)
        {
            services.TryAddSingleton<IHubHelper, HubHelper>();
        }
    }
}