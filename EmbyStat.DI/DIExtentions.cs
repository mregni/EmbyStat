using EmbyStat.Api.EmbyClient;
using EmbyStat.Api.EmbyClient.Cryptography;
using EmbyStat.Api.EmbyClient.Net;
using EmbyStat.Api.Github;
using EmbyStat.Api.Tvdb;
using EmbyStat.Api.WebSocketClient;
using EmbyStat.Common.Exceptions;
using EmbyStat.Common.Helpers;
using EmbyStat.Common.Models.Tasks.Interface;
using EmbyStat.Repositories;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services;
using EmbyStat.Services.Interfaces;
using EmbyStat.Tasks;
using Microsoft.Extensions.DependencyInjection;

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
        }

        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddTransient<IConfigurationService, ConfigurationService>();
            services.AddTransient<IPluginService, PluginService>();
            services.AddTransient<IEmbyService, EmbyService>();
            services.AddTransient<ITaskService, TaskService>();
            services.AddTransient<IMovieService, MovieService>();
            services.AddTransient<IPersonService, PersonService>();
            services.AddTransient<IShowService, ShowService>();
            services.AddTransient<ILogService, LogService>();
            services.AddTransient<ILanguageService, LanguageService>();
            services.AddTransient<IAboutService, AboutService>();
            services.AddTransient<IWebSocketService, WebSocketService>();
            services.AddTransient<IUpdateService, UpdateService>();
        }

        public static void RegisterRepositories(this IServiceCollection services)
        {
            services.AddTransient<IDatabaseInitializer, DatabaseInitializer>();

            services.AddTransient<IMovieRepository, MovieRepository>();
            services.AddTransient<IConfigurationRepository, ConfigurationRepository>();
            services.AddTransient<IPluginRepository, PluginRepository>();
            services.AddTransient<IServerInfoRepository, ServerInfoRepository>();
            services.AddTransient<IDriveRepository, DriveRepository>();
            services.AddTransient<IGenreRepository, GenreRepository>();
            services.AddTransient<IPersonRepository, PersonRepository>();
            services.AddTransient<IShowRepository, ShowRepository>();
            services.AddTransient<ICollectionRepository, CollectionRepository>();
            services.AddTransient<IStatisticsRepository, StatisticsRepository>();
            services.AddTransient<ILanguageRepository, LanguageRepository>();
            services.AddTransient<IEmbyStatusRepository, EmbyStatusRepository>();
        }

        public static void RegisterTasks(this IServiceCollection services)
        {
            services.AddSingleton<ITaskRepository, TaskRepository>();
            services.AddSingleton<ITaskManager, TaskManager>();
        }

        public static void RegisterClients(this IServiceCollection services)
        {
            services.AddTransient<IEmbyClient, EmbyClient>();
            services.AddTransient<ITvdbClient, TvdbClient>();
            services.AddTransient<IGithubClient, GithubClient>();
            services.AddTransient<IWebSocketClient, WebSocketClient>();
        }

        public static void RegisterHttp(this IServiceCollection services)
        {
            services.AddTransient<ICryptographyProvider, CryptographyProvider>();
            services.AddTransient<IJsonSerializer, JsonSerializer>();
            services.AddTransient<IAsyncHttpClient, HttpWebRequestClient>();
            services.AddTransient<IHttpWebRequestFactory, HttpWebRequestFactory>();
            services.AddTransient<BusinessExceptionFilterAttribute>();

        }
    }
}
