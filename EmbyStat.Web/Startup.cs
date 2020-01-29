using EmbyStat.Clients.Emby.Http;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Jobs;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rollbar;
using Rollbar.NetCore.AspNet;
using System;
using System.IO;
using System.Reflection;
using AutoMapper;
using EmbyStat.Common.Exceptions;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Hubs.Job;
using EmbyStat.Controllers;
using EmbyStat.DI;
using EmbyStat.Migrator;
using EmbyStat.Migrator.Interfaces;
using EmbyStat.Migrator.Migrations;
using EmbyStat.Services;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.MemoryStorage;
using Hangfire.RecurringJobExtensions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace EmbyStat.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment WebHostEnvironment { get; }
        public IApplicationBuilder ApplicationBuilder { get; set; }

        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            WebHostEnvironment = webHostEnvironment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<AppSettings>(Configuration);
            var appSettings = Configuration.Get<AppSettings>();

            RollbarLocator.RollbarInstance.Configure(new RollbarConfig(appSettings.Rollbar.AccessToken));

            services.AddRollbarLogger(loggerOptions =>
            {
                loggerOptions.Filter = (loggerName, logLevel) => logLevel >= (LogLevel)Enum.Parse(typeof(LogLevel), appSettings.Rollbar.LogLevel);
            });

            services
                .AddMvcCore(options => {
                    options.Filters.Add(new BusinessExceptionFilterAttribute());
                    options.EnableEndpointRouting = false;
                })
                .AddApplicationPart(Assembly.Load(new AssemblyName("EmbyStat.Controllers")))
                .AddApiExplorer()
                .AddAuthorization();

            services.AddHangfire(x =>
            {
                x.UseMemoryStorage();
                x.UseRecurringJob();
            });

            SetupDirectories(appSettings);

            services.AddAutoMapper(typeof(MapProfiles));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EmbyStat API", Version = "1.0"});
            });

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services.AddSignalR();
            services.AddCors();

            services.RegisterApplicationDependencies();
            services.AddHostedService<WebSocketService>();
            services.AddJsonMigrator(typeof(CreateUserSettings).Assembly);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            ApplicationBuilder = app;

            lifetime.ApplicationStarted.Register(PerformPostStartupFunctions);
            lifetime.ApplicationStopping.Register(PerformPreShutdownFunctions);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseRollbarMiddleware();

            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                WorkerCount = 1,
                SchedulePollingInterval = new TimeSpan(0, 0, 5),
                ServerTimeout = TimeSpan.FromDays(1),
                ShutdownTimeout = TimeSpan.FromDays(1),
                ServerName = "Main server",
                Queues = new[] { "main" }
            });

            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 2 });

            if (env.IsDevelopment())
            {
                app.UseHangfireDashboard("/hangfire",
                    new DashboardOptions
                    {
                        Authorization = new[] { new LocalRequestsOnlyAuthorizationFilter() }
                    });
            }

            app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            app.UseRouting();
            app.UseEndpoints(routes =>
            {
                routes.MapHub<JobHub>("/jobs-socket");
            });

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseMvc();

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });

        }

        private void SetupDirectories(AppSettings settings)
        {
            if (Directory.Exists(settings.Dirs.TempUpdate.GetLocalPath()))
            {
                Directory.Delete(settings.Dirs.TempUpdate.GetLocalPath(), true);
            }
        }

        private void PerformPostStartupFunctions()
        {
            using var serviceScope = ApplicationBuilder.ApplicationServices.CreateScope();
            var migrationRunner = serviceScope.ServiceProvider.GetService<IMigrationRunner>();
            migrationRunner.Migrate();
                
            var settingsService = serviceScope.ServiceProvider.GetService<ISettingsService>();
            var jobService = serviceScope.ServiceProvider.GetService<IJobService>();
            var embyClient = serviceScope.ServiceProvider.GetService<IEmbyClient>();
            var jobInitializer = serviceScope.ServiceProvider.GetService<IJobInitializer>();

            var settings = settingsService.GetAppSettings();
            
            settingsService.LoadUserSettingsFromFile();
            settingsService.CreateRollbarLogger();
            AddDeviceIdToConfig(settingsService);
            RemoveVersionFiles();
            jobService.ResetAllJobs();
            SetEmbyClientConfiguration(settingsService, embyClient);
            jobInitializer.Setup(settings.NoUpdates);
        }

        private void PerformPreShutdownFunctions()
        {
            using var serviceScope = ApplicationBuilder.ApplicationServices.CreateScope();
            var jobService = serviceScope.ServiceProvider.GetService<IJobService>();
            jobService.ResetAllJobs();
        }

        private void RemoveVersionFiles()
        {
            foreach (var file in Directory.GetFiles(WebHostEnvironment.ContentRootPath, "*.ver"))
            {
                File.Delete(file);
            }
        }

        private void AddDeviceIdToConfig(ISettingsService settingsService)
        {
            var userSettings = settingsService.GetUserSettings();

            if (userSettings.Id == null)
            {
                userSettings.Id = Guid.NewGuid();
                settingsService.SaveUserSettingsAsync(userSettings);
            }
        }

        private void SetEmbyClientConfiguration(ISettingsService settingsService, IEmbyClient embyClient)
        {
            settingsService.SetUpdateInProgressSettingAsync(false);
            var settings = settingsService.GetUserSettings();

            embyClient.SetDeviceInfo(settings.AppName, settings.Emby.AuthorizationScheme, settingsService.GetAppSettings().Version.ToCleanVersionString(), settings.Id.ToString());
            if (!string.IsNullOrWhiteSpace(settings.Emby.ApiKey))
            {
                embyClient.BaseUrl = settings.Emby.FullEmbyServerAddress;
                embyClient.ApiKey = settings.Emby.ApiKey;
            }
        }
    }
}
