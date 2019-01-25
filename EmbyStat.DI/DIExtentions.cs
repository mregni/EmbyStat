using EmbyStat.Clients.EmbyClient;
using EmbyStat.Clients.EmbyClient.Cryptography;
using EmbyStat.Clients.EmbyClient.Net;
using EmbyStat.Clients.Github;
using EmbyStat.Clients.Tvdb;
using EmbyStat.Clients.WebSocketClient;
using EmbyStat.Common.Exceptions;
using EmbyStat.Common.Helpers;
using EmbyStat.Common.Hubs;
using EmbyStat.Common.Hubs.Job;
using EmbyStat.Jobs;
using EmbyStat.Jobs.Jobs.Interfaces;
using EmbyStat.Jobs.Jobs.Maintenance;
using EmbyStat.Jobs.Jobs.Sync;
using EmbyStat.Jobs.Jobs.Updater;
using EmbyStat.Repositories;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services;
using EmbyStat.Services.Interfaces;
using EmbyStat.Sockets.EmbyClient;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EmbyStat.DI
{
    public static class DIExtentions
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
            services.TryAddTransient<IConfigurationService, ConfigurationService>();
            services.TryAddTransient<IPluginService, PluginService>();
            services.TryAddTransient<IEmbyService, EmbyService>();
            services.TryAddTransient<IMovieService, MovieService>();
            services.TryAddTransient<IPersonService, PersonService>();
            services.TryAddTransient<IShowService, ShowService>();
            services.TryAddTransient<ILogService, LogService>();
            services.TryAddTransient<ILanguageService, LanguageService>();
            services.TryAddTransient<IAboutService, AboutService>();
            services.TryAddSingleton<IWebSocketService, WebSocketService>();
            services.TryAddTransient<IUpdateService, UpdateService>();
            services.TryAddTransient<IJobService, JobService>();
        }

        public static void RegisterRepositories(this IServiceCollection services)
        {
            services.TryAddTransient<IDatabaseInitializer, DatabaseInitializer>();

            services.TryAddTransient<IMovieRepository, MovieRepository>();
            services.TryAddTransient<IConfigurationRepository, ConfigurationRepository>();
            services.TryAddTransient<IPluginRepository, PluginRepository>();
            services.TryAddTransient<IServerInfoRepository, ServerInfoRepository>();
            services.TryAddTransient<IDriveRepository, DriveRepository>();
            services.TryAddTransient<IGenreRepository, GenreRepository>();
            services.TryAddTransient<IPersonRepository, PersonRepository>();
            services.TryAddTransient<IShowRepository, ShowRepository>();
            services.TryAddTransient<ICollectionRepository, CollectionRepository>();
            services.TryAddTransient<IStatisticsRepository, StatisticsRepository>();
            services.TryAddTransient<ILanguageRepository, LanguageRepository>();
            services.TryAddTransient<IEmbyStatusRepository, EmbyStatusRepository>();
            services.TryAddTransient<IJobRepository, JobRepository>();
            services.TryAddTransient<IEventRepository, EventRepository>();
        }

        public static void RegisterJobs(this IServiceCollection services)
        {
            services.TryAddSingleton<IBackgroundJobClient, BackgroundJobClient>();
            services.TryAddTransient<IJobInitializer, JobInitializer>();

            services.TryAddSingleton<IDatabaseCleanupJob, DatabaseCleanupJob>();
            services.TryAddSingleton<IPingEmbyJob, PingEmbyJob>();
            services.TryAddSingleton<IMediaSyncJob, MediaSyncJob>();
            services.TryAddSingleton<ISmallSyncJob, SmallSyncJob>();
            services.TryAddSingleton<ICheckUpdateJob, CheckUpdateJob>();
        }

        public static void RegisterClients(this IServiceCollection services)
        {
            services.TryAddTransient<IEmbyClient, EmbyClient>();
            services.TryAddTransient<ITvdbClient, TvdbClient>();
            services.TryAddTransient<IGithubClient, GithubClient>();
            services.TryAddTransient<IEmbySocketClient, EmbySocketClient>();
        }

        public static void RegisterHttp(this IServiceCollection services)
        {
            services.TryAddTransient<ICryptographyProvider, CryptographyProvider>();
            services.TryAddTransient<IJsonSerializer, JsonSerializer>();
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