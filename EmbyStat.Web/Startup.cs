using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using AutoMapper;
using EmbyStat.Clients.Emby.Http;
using EmbyStat.Common.Exceptions;
using EmbyStat.Common.Extentions;
using EmbyStat.Common.Hubs.Job;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Controllers;
using EmbyStat.DI;
using EmbyStat.Jobs;
using EmbyStat.Repositories;
using EmbyStat.Services;
using EmbyStat.Services.Interfaces;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.MemoryStorage;
using Hangfire.RecurringJobExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Rollbar;
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
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<AppSettings>(Configuration);
            var config = Configuration.Get<AppSettings>();

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(config.ConnectionStrings.Main));

            services.AddHangfire(x =>
            {
                x.UseMemoryStorage();
                x.UseRecurringJob();
            });

            var settings = Configuration.Get<AppSettings>();
            SetupDirectories(settings);

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

            services.RegisterApplicationDependencies();
            services.AddHostedService<WebSocketService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime)
        {
            ApplicationBuilder = app;
            AddDeviceIdToConfig();

            lifetime.ApplicationStarted.Register(PerformPostStartupFunctions);
            lifetime.ApplicationStopping.Register(PerformPreShutdownFunctions);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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
            app.UseSignalR(routes =>
            {
                routes.MapHub<JobHub>("/jobs-socket");
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

        private void SetupDirectories(AppSettings settings)
        {
            if (Directory.Exists(settings.Dirs.TempUpdateDir))
            {
                Directory.Delete(settings.Dirs.TempUpdateDir, true);
            }
        }

        private void PerformPostStartupFunctions()
        {
            RemoveVersionFiles();
            ResetAllJobs();
            ResetConfiguration();
            SetEmbyClientConfiguration();
            InitializeTasks();
        }

        private void PerformPreShutdownFunctions()
        {
            ResetAllJobs();
        }

        private void RemoveVersionFiles()
        {
            foreach (var file in Directory.GetFiles(HostingEnvironment.ContentRootPath, "*.ver"))
            {
                File.Delete(file);
            }
        }

        private void ResetAllJobs()
        {
            using (var serviceScope = ApplicationBuilder.ApplicationServices.CreateScope())
            {
                var jobService = serviceScope.ServiceProvider.GetService<IJobService>();
                jobService.ResetAllJobs();
            }
        }

        private void ResetConfiguration()
        {
            using (var serviceScope = ApplicationBuilder.ApplicationServices.CreateScope())
            {
                var configurationService = serviceScope.ServiceProvider.GetService<ISettingsService>();
                configurationService.SetUpdateInProgressSetting(false);
            }
        }

        private void AddDeviceIdToConfig()
        {
            using (var serviceScope = ApplicationBuilder.ApplicationServices.CreateScope())
            {
                var settingsService = serviceScope.ServiceProvider.GetService<ISettingsService>();
                var userSettings = settingsService.GetUserSettings();

                if (userSettings.Id == null)
                {
                    userSettings.Id = Guid.NewGuid();
                    settingsService.SaveUserSettings(userSettings);
                }
            }
        }

        private void SetEmbyClientConfiguration()
        {
            using (var serviceScope = ApplicationBuilder.ApplicationServices.CreateScope())
            {
                var configurationService = serviceScope.ServiceProvider.GetService<ISettingsService>();
                configurationService.SetUpdateInProgressSetting(false);

                var embyClient = serviceScope.ServiceProvider.GetService<IEmbyClient>();
                var settingsService = serviceScope.ServiceProvider.GetService<ISettingsService>();
                var settings = settingsService.GetUserSettings();

                embyClient.SetDeviceInfo(settings.AppName, settings.Emby.AuthorizationScheme, settingsService.GetAppSettings().Version.ToCleanVersionString(), settings.Id.ToString());
                if (!string.IsNullOrWhiteSpace(settings.Emby.AccessToken))
                {
                    embyClient.SetAddressAndUser(settings.FullEmbyServerAddress, settings.Emby.AccessToken, settings.Emby.UserId);
                }
            }
        }

        private void InitializeTasks()
        {
            using (var serviceScope = ApplicationBuilder.ApplicationServices.CreateScope())
            {
                var taskInitializer = serviceScope.ServiceProvider.GetService<IJobInitializer>();
                taskInitializer.Setup();
            }
        }
    }
}
