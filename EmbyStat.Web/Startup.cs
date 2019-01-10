using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using AutoMapper;
using EmbyStat.Common.Exceptions;
using EmbyStat.Common.Hubs;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Controllers;
using EmbyStat.DI;
using EmbyStat.Repositories;
using EmbyStat.Services;
using EmbyStat.Services.Interfaces;
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

            Configuration = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }

		public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<AppSettings>(Configuration);
            var config = Configuration.Get<AppSettings>();

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(config.ConnectionStrings.Main));

            var settings = Configuration.Get<AppSettings>();
            SetupDirectories(settings);
            RemoveVersionFiles();

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

            services.RegisterApplicationDependencies();
            services.AddSingleton<IUpdateService, UpdateService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            ApplicationBuilder = app;
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

        private void SetupDirectories(AppSettings settings)
        {
            if (Directory.Exists(settings.Dirs.TempUpdateDir)) {
                Directory.Delete(settings.Dirs.TempUpdateDir, true);
            }
        }

        private void RemoveVersionFiles()
        {
            foreach (var file in Directory.GetFiles(HostingEnvironment.ContentRootPath, "*.ver"))
            {
                File.Delete(file);
            }
        }
    }
}
