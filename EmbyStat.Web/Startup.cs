using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using EmbyStat.Api.EmbyClient;
using EmbyStat.Api.EmbyClient.Cryptography;
using EmbyStat.Api.EmbyClient.Net;
using EmbyStat.Api.Github;
using EmbyStat.Api.Tvdb;
using EmbyStat.Api.WebSocketClient;
using EmbyStat.Common;
using EmbyStat.Common.Exceptions;
using EmbyStat.Common.Hubs;
using EmbyStat.Common.Models.Tasks.Interface;
using EmbyStat.Controllers;
using EmbyStat.Repositories;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services;
using EmbyStat.Services.Interfaces;
using EmbyStat.Tasks;
using EmbyStat.Tasks.Tasks;
using MediaBrowser.Model.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;

namespace EmbyStat.Web
{
	public class Startup
	{
		public IConfiguration Configuration { get; }
		public IHostingEnvironment HostingEnvironment { get; }
        public IApplicationBuilder ApplicationBuilder { get; set; }

        public Startup(IConfiguration configuration, IHostingEnvironment env)
		{
			HostingEnvironment = env;
			Configuration = configuration;

			var builder = new ConfigurationBuilder()
				.SetBasePath(HostingEnvironment.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddJsonFile($"appsettings.{HostingEnvironment.EnvironmentName}.json", optional: true)
				.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite("Data Source=data.db"));

		    services.AddOptions();
		    services.Configure<AppSettings>(Configuration);

            services
		        .AddMvcCore(options => { options.Filters.Add(new BusinessExceptionFilterAttribute()); })
		        .AddApplicationPart(Assembly.Load(new AssemblyName("EmbyStat.Controllers")))
		        .AddApiExplorer()
		        .AddJsonFormatters();

		    services.AddAutoMapper(typeof(MapProfiles));
            services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new Info { Title = "EmbyStat API", Version = "v1" });
			});

			services.AddSpaStaticFiles(configuration =>
			{
				configuration.RootPath = "ClientApp/dist";
            });

		    services.AddSignalR();
		    services.AddCors();

		    services.AddHostedService<WebSocketService>();

            var containerBuilder = new ContainerBuilder();
		    containerBuilder.Populate(services);
		    containerBuilder.RegisterType<ConfigurationService>().As<IConfigurationService>();
		    containerBuilder.RegisterType<PluginService>().As<IPluginService>();
		    containerBuilder.RegisterType<EmbyService>().As<IEmbyService>();
		    containerBuilder.RegisterType<TaskService>().As<ITaskService>();
		    containerBuilder.RegisterType<MovieService>().As<IMovieService>();
		    containerBuilder.RegisterType<PersonService>().As<IPersonService>();
		    containerBuilder.RegisterType<ShowService>().As<IShowService>();
		    containerBuilder.RegisterType<LogService>().As<ILogsService>();
		    containerBuilder.RegisterType<LanguageService>().As<ILanguageService>();
		    containerBuilder.RegisterType<AboutService>().As<IAboutService>();
		    containerBuilder.RegisterType<WebSocketService>().As<IWebSocketService>().SingleInstance();
		    containerBuilder.RegisterType<UpdateService>().As<IUpdateService>().SingleInstance();

            containerBuilder.RegisterType<MovieRepository>().As<IMovieRepository>();
            containerBuilder.RegisterType<ConfigurationRepository>().As<IConfigurationRepository>();
		    containerBuilder.RegisterType<PluginRepository>().As<IPluginRepository>();
		    containerBuilder.RegisterType<ServerInfoRepository>().As<IServerInfoRepository>();
		    containerBuilder.RegisterType<DriveRepository>().As<IDriveRepository>();
		    containerBuilder.RegisterType<GenreRepository>().As<IGenreRepository>();
		    containerBuilder.RegisterType<PersonRepository>().As<IPersonRepository>();
		    containerBuilder.RegisterType<ShowRepository>().As<IShowRepository>();
		    containerBuilder.RegisterType<CollectionRepository>().As<ICollectionRepository>();
		    containerBuilder.RegisterType<StatisticsRepository>().As<IStatisticsRepository>();
		    containerBuilder.RegisterType<LanguageRepository>().As<ILanguageRepository>();
		    containerBuilder.RegisterType<EmbyStatusRepository>().As<IEmbyStatusRepository>();

            containerBuilder.RegisterType<TaskRepository>().As<ITaskRepository>().SingleInstance();
            containerBuilder.RegisterType<TaskManager>().As<ITaskManager>().SingleInstance();
		    containerBuilder.RegisterType<EmbyClient>().As<IEmbyClient>();
		    containerBuilder.RegisterType<TvdbClient>().As<ITvdbClient>();
		    containerBuilder.RegisterType<GithubClient>().As<IGithubClient>();
            containerBuilder.RegisterType<WebSocketClient>().As<IWebSocketClient>();

            containerBuilder.RegisterType<CryptographyProvider>().As<ICryptographyProvider>();
		    containerBuilder.RegisterType<JsonSerializer>().As<IJsonSerializer>();
		    containerBuilder.RegisterType<HttpWebRequestClient>().As<IAsyncHttpClient>();
		    containerBuilder.RegisterType<HttpWebRequestFactory>().As<IHttpWebRequestFactory>();

		    containerBuilder.RegisterType<DatabaseInitializer>().As<IDatabaseInitializer>();
		    containerBuilder.RegisterType<BusinessExceptionFilterAttribute>();
            var container = containerBuilder.Build();
		    return new AutofacServiceProvider(container);
        }

	    public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
	    {
	        ApplicationBuilder = app;
            applicationLifetime.ApplicationStarted.Register(OnStarted);
            if (env.IsDevelopment())
			{
			    app.UseDeveloperExceptionPage();
            }

            app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            app.UseSignalR(routes =>
            {
                routes.MapHub<TaskHub>("/tasksignal");
            });

            app.UseSwagger();
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
			});

			app.UseStaticFiles();
			app.UseSpaStaticFiles();
            
			app.UseMvc();

			app.UseExceptionHandler(
				builder =>
				{
					builder.Run(
						async context =>
						{
							context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
							context.Response.ContentType = "application/json";
							var ex = context.Features.Get<IExceptionHandlerFeature>();
							if (ex != null)
							{
								var err = JsonConvert.SerializeObject(new { ex.Error.Message });
								await context.Response.Body.WriteAsync(Encoding.ASCII.GetBytes(err), 0, err.Length).ConfigureAwait(false);
							}
						});
				}
			);

			app.UseSpa(spa =>
			{
				spa.Options.SourcePath = "ClientApp";

				if (env.IsDevelopment())
				{
					spa.UseAngularCliServer(npmScript: "start");
				}
			});
        }

	    private void OnStarted()
	    {
	        var taskManager = ApplicationBuilder.ApplicationServices.GetService<ITaskManager>();

	        var tasks = new List<IScheduledTask>
	        {
	            new PingEmbyTask(ApplicationBuilder),
	            new SmallSyncTask(ApplicationBuilder),
                new MediaSyncTask(ApplicationBuilder),
                new DatabaseCleanupTask(ApplicationBuilder)
            };

	        taskManager.AddTasks(tasks);
        }
    }
}
