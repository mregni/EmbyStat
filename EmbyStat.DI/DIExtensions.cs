using EmbyStat.Clients.Emby.Http;
using EmbyStat.Clients.Emby.WebSocket;
using EmbyStat.Clients.GitHub;
using EmbyStat.Clients.Tvdb;
using EmbyStat.Common.Exceptions;
using EmbyStat.Common.Hubs.Job;
using EmbyStat.Common.Net;
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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
        }

        public static void RegisterServices(this IServiceCollection services)
        {
            services.TryAddTransient<IAboutService, AboutService>();
            services.TryAddTransient<IEmbyService, EmbyService>();
            services.TryAddTransient<IJobService, JobService>();
            services.TryAddTransient<ILanguageService, LanguageService>();
            services.TryAddTransient<ILogService, LogService>();
            services.TryAddTransient<IMovieService, MovieService>();
            services.TryAddTransient<IPersonService, PersonService>();
            services.TryAddTransient<ISessionService, SessionService>();
            services.TryAddSingleton<ISettingsService, SettingsService>();
            services.TryAddTransient<IShowService, ShowService>();
            services.TryAddTransient<IUpdateService, UpdateService>();
        }

        public static void RegisterRepositories(this IServiceCollection services)
        {
            services.TryAddTransient<IDbContext, DbContext>();
            services.TryAddTransient<IDatabaseInitializer, DatabaseInitializer>();

            services.TryAddTransient<IMovieRepository, MovieRepository>();
            services.TryAddTransient<IEmbyRepository, EmbyRepository>();
            services.TryAddTransient<IGenreRepository, GenreRepository>();
            services.TryAddTransient<IPersonRepository, PersonRepository>();
            services.TryAddTransient<IShowRepository, ShowRepository>();
            services.TryAddTransient<ICollectionRepository, CollectionRepository>();
            services.TryAddTransient<IStatisticsRepository, StatisticsRepository>();
            services.TryAddTransient<ILanguageRepository, LanguageRepository>();
            services.TryAddTransient<IJobRepository, JobRepository>();
            services.TryAddTransient<ISessionRepository, SessionRepository>();
        }

        public static void RegisterJobs(this IServiceCollection services)
        {
            services.TryAddSingleton<IBackgroundJobClient, BackgroundJobClient>();
            services.TryAddTransient<IJobInitializer, JobInitializer>();

            services.TryAddTransient<IDatabaseCleanupJob, DatabaseCleanupJob>();
            services.TryAddTransient<IPingEmbyJob, PingEmbyJob>();
            services.TryAddTransient<IMediaSyncJob, MediaSyncJob>();
            services.TryAddTransient<ISmallSyncJob, SmallSyncJob>();
            services.TryAddTransient<ICheckUpdateJob, CheckUpdateJob>();
        }

        public static void RegisterClients(this IServiceCollection services)
        {
            services.TryAddSingleton<IEmbyClient, EmbyClient>();
            services.TryAddTransient<ITvdbClient, TvdbClient>();
            services.TryAddTransient<IGithubClient, GithubClient>();

            services.TryAddSingleton<IWebSocketApi, WebSocketApi>();
            services.TryAddSingleton<IClientWebSocket, ClientWebSocket>();
        }

        public static void RegisterHttp(this IServiceCollection services)
        {
            services.TryAddTransient<IAsyncHttpClient, HttpWebRequestClient>();
            services.TryAddTransient<IHttpWebRequestFactory, HttpWebRequestFactory>();
            services.TryAddTransient<BusinessExceptionFilterAttribute>();
        }

        public static void RegisterSignalR(this IServiceCollection services)
        {
            services.TryAddSingleton<IJobHubHelper, JobHubHelper>();
        }
    }
}