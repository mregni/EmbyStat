using EmbyStat.Common.Models.Settings;
using EmbyStat.Jobs;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rollbar;
using Rollbar.NetCore.AspNet;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EmbyStat.Clients.Base;
using EmbyStat.Clients.Base.Api;
using EmbyStat.Common;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Exceptions;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Hubs;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Controllers;
using EmbyStat.Controllers.Middleware;
using EmbyStat.DI;
using EmbyStat.Migrator;
using EmbyStat.Migrator.Interfaces;
using EmbyStat.Migrator.Migrations;
using EmbyStat.Repositories;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.MemoryStorage;
using Hangfire.RecurringJobExtensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Refit;

namespace EmbyStat.Web;

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
        services.RegisterApplicationDependencies();

        RollbarLocator.RollbarInstance.Configure(new RollbarConfig(appSettings.Rollbar.AccessToken));

        services.AddRollbarLogger(loggerOptions =>
        {
            loggerOptions.Filter = (_, logLevel) => logLevel >= (LogLevel)Enum.Parse(typeof(LogLevel), appSettings.Rollbar.LogLevel);
        });

        services.AddCors(b => b.AddPolicy("default"
            , builder => {
                builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed(_ => true)
                    .AllowCredentials();
            }));
        
        services
            .AddMvcCore(options => {
                options.Filters.Add(new BusinessExceptionFilterAttribute());
                options.EnableEndpointRouting = false;
            })
            .AddApplicationPart(Assembly.Load(new AssemblyName("EmbyStat.Controllers")))
            .AddApiExplorer()
            .AddAuthorization();

        SetupDirectories(appSettings);

        services.AddAutoMapper(typeof(MapProfiles));
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "EmbyStat API", Version = "1.0"});
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Scheme = "bearer",
                Description = "Please insert JWT token into field"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
        });

        services.AddRefitClient<IMediaServerApi>();

        services.AddSpaStaticFiles(configuration =>
        {
            configuration.RootPath = "dist";
        });

        services.AddSignalR();
        services.AddDbContext<EsDbContext>();

        services.AddAuthorization(options =>
        {
            options.AddPolicy("ApiUser", policy => policy.RequireRole(Constants.Roles.User));
            options.AddPolicy("ApiAdmin", policy => policy.RequireRole(Constants.Roles.Admin));
        });
            
        services.AddIdentity<EmbyStatUser, IdentityRole>(options => {
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.SignIn.RequireConfirmedAccount = false;
                options.User.RequireUniqueEmail = false;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<EsDbContext>()
            .AddDefaultTokenProviders();
            
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.ClaimsIssuer = appSettings.Jwt.Issuer;
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(appSettings.Jwt.Key)),

                    ValidateIssuer = true,
                    ValidIssuer = appSettings.Jwt.Issuer,

                    ValidateAudience = true,
                    ValidAudience = appSettings.Jwt.Audience,

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
                x.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("AccessToken-Expired", "true");
                        }

                        return Task.CompletedTask;
                    }
                };
            });
            
        services.AddHttpContextAccessor();

        //services.AddHostedService<WebSocketService>();
        services.AddJsonMigrator(typeof(CreateUserSettings).Assembly);
        
        services.AddHangfire(x =>
        {
            x.UseMemoryStorage();
            x.UseRecurringJob();
        });
        services.AddHangfireServer(options =>
        {
            options.WorkerCount = 1;
            options.SchedulePollingInterval = new TimeSpan(0, 0, 5);
            options.ServerTimeout = TimeSpan.FromDays(1);
            options.ShutdownTimeout = TimeSpan.FromDays(1);
            options.ServerName = "Main server";
            options.Queues = new[] {"main"};
        });
        
        services.AddHttpLogging(logging =>
        {
            logging.LoggingFields = HttpLoggingFields.All;
            logging.MediaTypeOptions.AddText("application/javascript");
            logging.RequestBodyLogLimit = 4096;
            logging.ResponseBodyLogLimit = 4096;
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
    {
        ApplicationBuilder = app;

        lifetime.ApplicationStarted.Register(PerformPostStartup);
        lifetime.ApplicationStopping.Register(PerformPreShutdown);

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        //app.UseRollbarMiddleware();

        GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 2 });

        if (env.IsDevelopment())
        {
            app.UseHangfireDashboard("/hangfire",
                new DashboardOptions
                {
                    Authorization = new[] { new LocalRequestsOnlyAuthorizationFilter() }
                });
        }
            
        app.UseCors("default");
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(routes =>
        {
            routes.MapHub<EmbyStatHub>("/hub");
        });

        app.UseSwagger();
        app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });

        app.UseStaticFiles();
        app.UseSpaStaticFiles();

        app.UseMiddleware<RequestLoggingMiddleware>();
        app.UseMvc();

        app.UseSpa(spa =>
        {
            if (env.IsDevelopment())
            {
                spa.Options.SourcePath = "ClientApp";
                spa.UseReactDevelopmentServer("start");
            }
            else
            {
                spa.Options.SourcePath = "dist";
            }
        });

    }

    private static void SetupDirectories(AppSettings settings)
    {
        if (Directory.Exists(settings.Dirs.TempUpdate.GetLocalPath()))
        {
            Directory.Delete(settings.Dirs.TempUpdate.GetLocalPath(), true);
        }
    }

    private void PerformPostStartup()
    {
        using var serviceScope = ApplicationBuilder.ApplicationServices.CreateScope();
        var migrationRunner = serviceScope.ServiceProvider.GetService<IMigrationRunner>();
        migrationRunner?.Migrate();

        var settingsService = serviceScope.ServiceProvider.GetService<ISettingsService>();
        var accountService = serviceScope.ServiceProvider.GetService<IAccountService>();
        var clientStrategy = serviceScope.ServiceProvider.GetService<IClientStrategy>();
        var jobInitializer = serviceScope.ServiceProvider.GetService<IJobInitializer>();
        
        if (settingsService != null)
        {
            var settings = settingsService.GetAppSettings();
            settingsService?.LoadUserSettingsFromFile();
            accountService?.CreateRoles();
            settingsService.CreateRollbarLogger();
            AddDeviceIdToConfig(settingsService);
            SetMediaServerClientConfiguration(settingsService, clientStrategy);
            jobInitializer?.Setup(settings.NoUpdates);
        }
        
        RemoveVersionFiles();
        
        var jobService = serviceScope.ServiceProvider.GetService<IJobService>();
        jobService?.ResetAllJobs();
    }

    private void PerformPreShutdown()
    {
        using var serviceScope = ApplicationBuilder.ApplicationServices.CreateScope();
        var jobService = serviceScope.ServiceProvider.GetService<IJobService>();
        jobService?.ResetAllJobs();
    }

    private void RemoveVersionFiles()
    {
        foreach (var file in Directory.GetFiles(WebHostEnvironment.ContentRootPath, "*.ver"))
        {
            File.Delete(file);
        }
    }

    private static void AddDeviceIdToConfig(ISettingsService settingsService)
    {
        var userSettings = settingsService.GetUserSettings();

        if (userSettings.Id != null)
        {
            return;
        }
        userSettings.Id = Guid.NewGuid();
        settingsService.SaveUserSettingsAsync(userSettings);
    }

    private static void SetMediaServerClientConfiguration(ISettingsService settingsService, IClientStrategy clientStrategy)
    {
        settingsService.SetUpdateInProgressSettingAsync(false);
        var settings = settingsService.GetUserSettings();

        var mediaServerType = settings.MediaServer?.Type ?? ServerType.Emby;
        var mediaServerClient = clientStrategy.CreateHttpClient(mediaServerType);

        mediaServerClient.SetDeviceInfo(
            settings.AppName, 
            settings.MediaServer?.AuthorizationScheme ?? string.Empty, 
            settingsService.GetAppSettings().Version.ToCleanVersionString(), 
            settings.Id.ToString(),
            settings.MediaServer?.UserId ?? string.Empty);

        if (!string.IsNullOrWhiteSpace(settings.MediaServer?.ApiKey))
        {
            mediaServerClient.BaseUrl = settings.MediaServer.Address;
            mediaServerClient.ApiKey = settings.MediaServer.ApiKey;
        }
    }
}