using System;
using System.Net;
using System.Text;
using AutoMapper;
using EmbyStat.Common.Exceptions;
using EmbyStat.Controllers.Helpers;
using EmbyStat.Repositories;
using EmbyStat.Repositories.Config;
using EmbyStat.Repositories.EmbyPlugin;
using EmbyStat.Repositories.EmbyServerInfo;
using EmbyStat.Services.Config;
using EmbyStat.Services.Emby;
using EmbyStat.Services.EmbyClient;
using EmbyStat.Services.EmbyClient.Cryptography;
using EmbyStat.Services.EmbyClient.Net;
using EmbyStat.Services.Plugin;
using Hangfire;
using Hangfire.MemoryStorage;
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

		public Startup(IConfiguration configuration, IHostingEnvironment env)
		{
			HostingEnvironment = env;
			Configuration = configuration;

			var builder = new ConfigurationBuilder()
				.SetBasePath(HostingEnvironment.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appsettings.{HostingEnvironment.EnvironmentName}.json", optional: true)
				.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddHangfire(config => config.UseMemoryStorage());

			services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite("Data Source=data.db"));

			services.AddMvc(options =>
			{
				options.Filters.Add(new BusinessExceptionFilterAttribute());
			});
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
			});

			services.AddSpaStaticFiles(configuration =>
			{
				configuration.RootPath = "ClientApp/dist";
			});

			services.AddScoped<IConfigurationService, ConfigurationService>();
			services.AddScoped<IPluginService, PluginService>();
			services.AddScoped<IPluginService, PluginService>();

			services.AddScoped<IConfigurationRepository, PluginRepository>();
			services.AddScoped<IEmbyPluginRepository, EmbyPluginRepository>();
			services.AddScoped<IEmbyServerInfoRepository, EmbyServerInfoRepository>();

			services.AddScoped<IEmbyClient, EmbyClient>();
			services.AddScoped<ICryptographyProvider, CryptographyProvider>();
			services.AddScoped<IJsonSerializer, NewtonsoftJsonSerializer>();
			services.AddScoped<IAsyncHttpClient, HttpWebRequestClient>();
			services.AddScoped<IHttpWebRequestFactory, HttpWebRequestFactory>();

			services.AddTransient<IDatabaseInitializer, DatabaseInitializer>();
			services.AddScoped<BusinessExceptionFilterAttribute>();
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			Mapper.Initialize(cfg =>
			{
				cfg.AddProfile<MapProfiles>();
			});

			app.UseHangfireServer();
			app.UseHangfireDashboard();
			// Enable middleware to serve generated Swagger as a JSON endpoint.
			app.UseSwagger();

			// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
			});

			app.UseStaticFiles();
			app.UseSpaStaticFiles();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller}/{action=Index}/{id?}");
			});

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
				// To learn more about options for serving an Angular SPA from ASP.NET Core,
				// see https://go.microsoft.com/fwlink/?linkid=864501

				spa.Options.SourcePath = "ClientApp";

				if (env.IsDevelopment())
				{
					spa.UseAngularCliServer(npmScript: "start");
				}
			});
		}

		private void onShutdown()
		{
			
		}
	}
}
