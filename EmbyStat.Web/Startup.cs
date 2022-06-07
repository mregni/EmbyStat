using EmbyStat.Jobs;
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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using EmbyStat.Clients.Base;
using EmbyStat.Clients.Base.Api;
using EmbyStat.Common;
using EmbyStat.Common.Exceptions;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Configuration;
using EmbyStat.Configuration.Interfaces;
using EmbyStat.Controllers;
using EmbyStat.Controllers.Middleware;
using EmbyStat.Core.Account;
using EmbyStat.Core.Account.Interfaces;
using EmbyStat.Core.DataStore;
using EmbyStat.Core.Hubs;
using EmbyStat.Core.Jobs.Interfaces;
using EmbyStat.DI;
using EmbyStat.Migrator;
using EmbyStat.Migrator.Interfaces;
using EmbyStat.Migrator.Migrations;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.MemoryStorage;
using Hangfire.RecurringJobExtensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Refit;
using Rollbar.DTOs;

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
        services.Configure<Config>(Configuration);
        var configuration = Configuration.Get<Config>();
        services.RegisterApplicationDependencies();
        services.AddHttpContextAccessor();
        
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
        
        ConfigureRollbarInfrastructure(configuration);
        
        services.AddRollbarLogger(loggerOptions =>
        {
            loggerOptions.Filter = loggerOptions.Filter = (_, loglevel) => loglevel >= LogLevel.Error;
        });

        services.AddSpaStaticFiles(configuration =>
        {
            configuration.RootPath = "dist";
        });

        services.AddSignalR();
        var dbPath = Path.Combine(configuration.SystemConfig.Dirs.Data, "SqliteData.db");
        services.AddDbContext<EsDbContext>(options =>
        {
            options.EnableDetailedErrors();
            options.UseSqlite($"Data Source={dbPath}", x => x.MigrationsAssembly("EmbyStat.Migrations"));
        });

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
                x.ClaimsIssuer = configuration.SystemConfig.Jwt.Issuer;
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration.SystemConfig.Jwt.Key)),

                    ValidateIssuer = true,
                    ValidIssuer = configuration.SystemConfig.Jwt.Issuer,

                    ValidateAudience = true,
                    ValidAudience = configuration.SystemConfig.Jwt.Audience,

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
            
        //services.AddHostedService<WebSocketService>();
        services.AddJsonMigrator(typeof(DummyMigration).Assembly);
        
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

        app.UseRollbarMiddleware();

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

    private void PerformPostStartup()
    {
        using var serviceScope = ApplicationBuilder.ApplicationServices.CreateScope();
        var db = serviceScope.ServiceProvider.GetRequiredService<EsDbContext>();
        db.Database.Migrate();
        
        var migrationRunner = serviceScope.ServiceProvider.GetService<IMigrationRunner>();
        migrationRunner?.Migrate();
  
        var configurationService = serviceScope.ServiceProvider.GetService<IConfigurationService>();
        var accountService = serviceScope.ServiceProvider.GetService<IAccountService>();
        var clientStrategy = serviceScope.ServiceProvider.GetService<IClientStrategy>();
        var jobInitializer = serviceScope.ServiceProvider.GetService<IJobInitializer>();

        if (configurationService != null)
        {
            var config = configurationService.Get();
            SetupDirectories(config);
            accountService?.CreateRoles();
            SetMediaServerClientConfiguration(configurationService, clientStrategy);
            jobInitializer?.Setup(config.SystemConfig.CanUpdate);
        }
        
        RemoveVersionFiles();
        
        var jobService = serviceScope.ServiceProvider.GetService<IJobService>();
        jobService?.ResetAllJobs();
    }
    
    private static void SetupDirectories(Config settings)
    {
        if (Directory.Exists(settings.SystemConfig.Dirs.TempUpdate.GetLocalPath()))
        {
            Directory.Delete(settings.SystemConfig.Dirs.TempUpdate.GetLocalPath(), true);
        }
    }
    
    private void ConfigureRollbarInfrastructure(Config configuration)
    {
        var config = new RollbarInfrastructureConfig(
            configuration.SystemConfig.Rollbar.AccessToken, 
            configuration.SystemConfig.Rollbar.Environment
        );

        var offlineStoreOptions = new RollbarOfflineStoreOptions
        {
            EnableLocalPayloadStore = false
        };
        config.RollbarOfflineStoreOptions.Reconfigure(offlineStoreOptions);

        var telemetryOptions = new RollbarTelemetryOptions(true, 3);
        config.RollbarTelemetryOptions.Reconfigure(telemetryOptions);

        var dataSecurityOptions = 
            new RollbarDataSecurityOptions
            {
                IpAddressCollectionPolicy = IpAddressCollectionPolicy.DoNotCollect,
                PersonDataCollectionPolicies = PersonDataCollectionPolicies.None,
                ScrubFields = new[] {
                    "user_password",
                    "secret",
                }
            };
        config.RollbarLoggerConfig.RollbarDataSecurityOptions.Reconfigure(dataSecurityOptions);

        var payloadAdditionOptions = 
            new RollbarPayloadAdditionOptions
            {
                CodeVersion = configuration.SystemConfig.Version,
                Server = new Server
                {
                    {"Framework", RuntimeInformation.FrameworkDescription},
                    {"OS", RuntimeInformation.OSDescription}
                }
            };
        config.RollbarLoggerConfig.RollbarPayloadAdditionOptions.Reconfigure(payloadAdditionOptions);

        RollbarInfrastructure.Instance.Init(config);
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

    private static void SetMediaServerClientConfiguration(IConfigurationService configurationService, IClientStrategy clientStrategy)
    {
        configurationService.SetUpdateInProgressSettingAsync(false);
        var config = configurationService.Get();

        var mediaServerType = config.UserConfig.MediaServer.Type;
        var mediaServerClient = clientStrategy.CreateHttpClient(mediaServerType);

        mediaServerClient.SetDeviceInfo(
            config.SystemConfig.AppName, 
            config.UserConfig.MediaServer.AuthorizationScheme,
            config.SystemConfig.Version.ToCleanVersionString(), 
            config.SystemConfig.Id.ToString(),
            config.UserConfig.MediaServer.UserId);

        if (!string.IsNullOrWhiteSpace(config.UserConfig.MediaServer?.ApiKey))
        {
            mediaServerClient.BaseUrl = config.UserConfig.MediaServer.Address;
            mediaServerClient.ApiKey = config.UserConfig.MediaServer.ApiKey;
        }
    }
}