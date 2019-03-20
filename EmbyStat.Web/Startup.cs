using EmbyStat.Clients.Emby.Http;
using EmbyStat.Common.Extentions;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Jobs;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rollbar;
using Rollbar.NetCore.AspNet;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using AutoMapper;
using EmbyStat.Common.Exceptions;
using EmbyStat.Common.Hubs.Job;
using EmbyStat.Controllers;
using EmbyStat.DI;
using EmbyStat.Repositories;
using EmbyStat.Services;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.MemoryStorage;
using Hangfire.RecurringJobExtensions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            ConfigureRollbarSingleton(appSettings);
            services.AddRollbarLogger(loggerOptions =>
            {
                loggerOptions.Filter = (loggerName, loglevel) => loglevel >= LogLevel.Trace;
            });

            services
                .AddMvcCore(/*options => { options.Filters.Add(new BusinessExceptionFilterAttribute()); }*/)
                .AddApplicationPart(Assembly.Load(new AssemblyName("EmbyStat.Controllers")))
                .AddApiExplorer()
                .AddJsonFormatters()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(appSettings.ConnectionStrings.Main));

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

        private void ConfigureRollbarSingleton(AppSettings appSettings)
        {
            const string rollbarAccessToken = "1b5400a805f94eaca69c40ea0ad8cbde";

            RollbarLocator.RollbarInstance.Configure(new RollbarConfig(rollbarAccessToken)
            {
                LogLevel = ErrorLevel.Critical,
                Environment = "RollbarENV",
                Enabled = true,
                MaxReportsPerMinute = 10,
                Transform = payload =>
                {
                    payload.Data.CodeVersion = appSettings.Version.ToCleanVersionString();
                }
            }).InternalEvent += Startup_InternalEvent;
        }

        private void Startup_InternalEvent(object sender, RollbarEventArgs e)
        {
            
        }

        private void SetupDirectories(AppSettings settings)
        {
            if (Directory.Exists(settings.Dirs.TempUpdateDir))
            {
                Directory.Delete(settings.Dirs.TempUpdateDir, true);
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

                AddDeviceIdToConfig(settingsService);
                RemoveVersionFiles();
                ResetAllJobs(jobService);
                ResetConfiguration(settingsService);
                SetEmbyClientConfiguration(settingsService, embyClient);
                InitializeTasks(jobInitializer);
            }
        }

        private void PerformPreShutdownFunctions()
        {
            using (var serviceScope = ApplicationBuilder.ApplicationServices.CreateScope())
            {
                var jobService = serviceScope.ServiceProvider.GetService<IJobService>();

                ResetAllJobs(jobService);
            }
        }

        private void RemoveVersionFiles()
        {
            foreach (var file in Directory.GetFiles(HostingEnvironment.ContentRootPath, "*.ver"))
            {
                File.Delete(file);
            }
        }

        private void ResetAllJobs(IJobService jobService)
        {
            jobService.ResetAllJobs();
        }

        private void ResetConfiguration(ISettingsService settingsService)
        {
            settingsService.SetUpdateInProgressSetting(false);
        }

        private void AddDeviceIdToConfig(ISettingsService settingsService)
        {
            var userSettings = settingsService.GetUserSettings();

            if (userSettings.Id == null)
            {
                userSettings.Id = Guid.NewGuid();
                settingsService.SaveUserSettings(userSettings);
            }
        }

        private void SetEmbyClientConfiguration(ISettingsService settingsService, IEmbyClient embyClient)
        {
            settingsService.SetUpdateInProgressSetting(false);
            var settings = settingsService.GetUserSettings();

            embyClient.SetDeviceInfo(settings.AppName, settings.Emby.AuthorizationScheme, settingsService.GetAppSettings().Version.ToCleanVersionString(), settings.Id.ToString());
            if (!string.IsNullOrWhiteSpace(settings.Emby.AccessToken))
            {
                embyClient.SetAddressAndUser(settings.FullEmbyServerAddress, settings.Emby.AccessToken, settings.Emby.UserId);
            }
        }

        private void InitializeTasks(IJobInitializer jobInitializer)
        {
            jobInitializer.Setup();
        }
    }
}
