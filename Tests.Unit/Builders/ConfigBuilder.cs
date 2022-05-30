using System;
using EmbyStat.Common.Enums;
using EmbyStat.Configuration;

namespace Tests.Unit.Builders;

public class ConfigBuilder
{
    private readonly Config _config;

    public ConfigBuilder()
    {
        _config = new Config
        {
            SystemConfig = new SystemConfig
            {
                Id = Guid.NewGuid(),
                AppName = "EmbyStat",
                AutoUpdate = false,
                UpdateInProgress = false,
                UpdateTrain = UpdateTrain.Beta,
                Migration = 0,
                ProcessName = "EmbyStat",
                UpdatesDisabled = false,
                Jwt = new Jwt
                {
                    Audience = "jwt-audience",
                    Issuer = "embystat",
                    Key = "xxxxxxxxxx",
                    AccessExpireMinutes = 120
                },
                Dirs = new Dirs {
                    Data = "data-dir", 
                    Logs = "log-dir",
                    Updater = "updater",
                    TempUpdate = "update-files"
                },
                Rollbar = new EmbyStat.Configuration.Rollbar(),
                Updater = new Updater
                {
                    Dll = "updater.dll",
                    BetaString = "-beta",
                    DevString = "-dev",
                    UpdateAsset = "asset",
                    GitHubUrl = "https://github.com"
                }
            },
            UserConfig = new UserConfig
            {
                KeepLogsCount = 10,
                Language = "en-US",
                ToShortMovie = 10,
                WizardFinished = true,
                LogLevel = 0,
                EnableRollbarLogging = true,
                ToShortMovieEnabled = false,
                MediaServer = new MediaServerSettings
                {
                    ApiKey = "1234567980",
                    Name = "ServerName",
                    AuthorizationScheme = "MediaBrowser",
                    Address = "https://localhost:8097",
                },
                Tmdb = new TmdbSettings
                {
                    LastUpdate = new DateTime(2019, 1, 1),
                    ApiKey = "ABCDE"
                },
                Hosting = new Hosting()
                {
                    Port = 6555,
                    Url = "*",
                    SslEnabled = false,
                    SslPort = 6556,
                    SslCertPassword = "test",
                    SslCertPath = "boe.ctx"
                }
            }
        };
    }

    public ConfigBuilder WithDeviceId(Guid id)
    {
        _config.SystemConfig.Id = id;
        return this;
    }

    public ConfigBuilder WithDataDir(string path)
    {
        _config.SystemConfig.Dirs.Data = path;
        return this;
    }
    
    public ConfigBuilder WithLogsDir(string path)
    {
        _config.SystemConfig.Dirs.Logs = path;
        return this;
    }

    public ConfigBuilder WithLogCount(int logCount)
    {
        _config.UserConfig.KeepLogsCount = logCount;
        return this;
    }
    
    public ConfigBuilder WithMediaServerAddress(string address)
    {
        _config.UserConfig.MediaServer.Address = address;
        return this;
    }
    
    public ConfigBuilder WithMediaServerApiKey(string key)
    {
        _config.UserConfig.MediaServer.ApiKey = key;
        return this;
    }
    
    public ConfigBuilder WithTmdbApiKey(string key)
    {
        _config.UserConfig.Tmdb.ApiKey = key;
        return this;
    }

    public ConfigBuilder EnableToShortMovies(bool state, int length)
    {
        _config.UserConfig.ToShortMovieEnabled = state;
        _config.UserConfig.ToShortMovie = length;
        return this;
    }

    public ConfigBuilder WithRollbarSettings(bool state)
    {
        _config.UserConfig.EnableRollbarLogging = state;
        return this;
    }

    public ConfigBuilder WithAutoUpdate(bool state)
    {
        _config.SystemConfig.UpdatesDisabled = state;
        return this;
    }

    public ConfigBuilder WithLanguage(string language)
    {
        _config.UserConfig.Language = language;
        return this;
    }

    public ConfigBuilder WithMigration(int migration)
    {
        _config.SystemConfig.Migration = migration;
        return this;
    }

    public ConfigBuilder WithAppName(string name)
    {
        _config.SystemConfig.AppName = name;
        return this;
    }

    public ConfigBuilder WithMediaServerType(ServerType type)
    {
        _config.UserConfig.MediaServer.Type = type;
        return this;
    }
    
    public Config Build()
    {
        return _config;
    }
}