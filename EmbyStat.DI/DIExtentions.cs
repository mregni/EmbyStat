using EmbyStat.Api.EmbyClient;
using EmbyStat.Api.EmbyClient.Cryptography;
using EmbyStat.Api.EmbyClient.Net;
using EmbyStat.Api.Github;
using EmbyStat.Api.Tvdb;
using EmbyStat.Api.WebSocketClient;
using EmbyStat.Common.Exceptions;
using EmbyStat.Common.Helpers;
using EmbyStat.Repositories;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services;
using EmbyStat.Services.Interfaces;
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
            services.RegisterTasks();
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
            services.TryAddTransient<IWebSocketService, WebSocketService>();
            services.TryAddTransient<IUpdateService, UpdateService>();
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
            services.TryAddTransient<ITaskRepository, TaskRepository>();
        }

        public static void RegisterTasks(this IServiceCollection services)
        {
            services.AddSingleton<ITaskRepository, TaskRepository>();
            //services.TryAddTransient<IBackgroundJobClient, BackgroundJobClient>();
            //services.TryAddTransient<ITaskInitializer, TaskInitializer>();

            //services.TryAddTransient<IDatabaseCleanupTask, DatabaseCleanupTask>();
            //services.TryAddTransient<IPingEmbyTask, PingEmbyTask>();
            //services.TryAddTransient<IMediaSyncTask, MediaSyncTask>();
            //services.TryAddTransient<ISmallSyncTask, SmallSyncTask>();
            //services.TryAddTransient<ICheckUpdateTask, CheckUpdateTask>();
        }

        public static void RegisterClients(this IServiceCollection services)
        {
            services.TryAddTransient<IEmbyClient, EmbyClient>();
            services.TryAddTransient<ITvdbClient, TvdbClient>();
            services.TryAddTransient<IGithubClient, GithubClient>();
            services.TryAddTransient<IWebSocketClient, WebSocketClient>();
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
            //services.TryAddSingleton<IHubHelper, HubHelper>();
        }
    }
}