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
using Swashbuckle.AspNetCore.Swagger;
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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Logging;

namespace EmbyStat.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }
        public IApplicationBuilder ApplicationBuilder { get; set; }

        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            HostingEnvironment = hostingEnvironment;
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
                .AddMvcCore(options => { options.Filters.Add(new BusinessExceptionFilterAttribute()); })
                .AddApplicationPart(Assembly.Load(new AssemblyName("EmbyStat.Controllers")))
                .AddApiExplorer()
                .AddJsonFormatters()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddHangfire(x =>
            {
                x.UseMemoryStorage();
                x.UseRecurringJob();
            });

            SetupDirectories(appSettings);

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

            services.RegisterApplicationDependencies();
            services.AddHostedService<WebSocketService>();
            services.AddJsonMigrator(typeof(CreateUserSettings).Assembly);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime)
        {
            ApplicationBuilder = app;

            lifetime.ApplicationStarted.Register(PerformPostStartupFunctions);
            lifetime.ApplicationStopping.Register(PerformPreShutdownFunctions);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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
            app.UseSignalR(routes => { routes.MapHub<JobHub>("/jobs-socket"); });

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
            if (Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), settings.Dirs.TempUpdateDir)))
            {
                Directory.Delete(Path.Combine(Directory.GetCurrentDirectory(), settings.Dirs.TempUpdateDir), true);
            }
        }

        private void PerformPostStartupFunctions()
        {
            using (var serviceScope = ApplicationBuilder.ApplicationServices.CreateScope())
            {
                var settingsService = serviceScope.ServiceProvider.GetService<ISettingsService>();
                var jobService = serviceScope.ServiceProvider.GetService<IJobService>();
                var embyClient = serviceScope.ServiceProvider.GetService<IEmbyClient>();
                var jobInitializer = serviceScope.ServiceProvider.GetService<IJobInitializer>();
                var migrationRunner = serviceScope.ServiceProvider.GetService<IMigrationRunner>();

                migrationRunner.Migrate();
                settingsService.LoadUserSettingsFromFile();
                settingsService.CreateRollbarLogger();
                AddDeviceIdToConfig(settingsService);
                RemoveVersionFiles();
                jobService.ResetAllJobs();
                settingsService.SetUpdateInProgressSettingAsync(false);
                SetEmbyClientConfiguration(settingsService, embyClient);
                jobInitializer.Setup();
            }
        }

        private void PerformPreShutdownFunctions()
        {
            using (var serviceScope = ApplicationBuilder.ApplicationServices.CreateScope())
            {
                var jobService = serviceScope.ServiceProvider.GetService<IJobService>();
                jobService.ResetAllJobs();
            }
        }

        private void RemoveVersionFiles()
        {
            foreach (var file in Directory.GetFiles(HostingEnvironment.ContentRootPath, "*.ver"))
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
            if (!string.IsNullOrWhiteSpace(settings.Emby.AccessToken))
            {
                embyClient.SetAddressAndUser(settings.FullEmbyServerAddress, settings.Emby.AccessToken, settings.Emby.UserId);
            }
        }
    }
}
