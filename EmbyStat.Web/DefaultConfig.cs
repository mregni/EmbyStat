using System;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Generators;
using EmbyStat.Configuration;

namespace EmbyStat.Web;

public static class DefaultConfig
{
    public static Config Default = new()
    {
        SystemConfig = new SystemConfig
        {
            Dirs = new Dirs
            {
                TempUpdate = "updateFiles",
                Updater = "updater"
            },
            Jwt = new Jwt
            {
                Audience = "user",
                Issuer = "embystat-admin",
                AccessExpireMinutes = 120,
                Key = KeyGenerator.GetUniqueKey(120)
            },
            AutoUpdate = true,
            UpdateTrain = UpdateTrain.Release,
            ProcessName = "EmbyStat",
            AppName = "EmbyStat",
            Id = Guid.NewGuid(),
            Migration = 0,
            UpdateInProgress = false,
            CanUpdate = true,
            Rollbar = new Configuration.Rollbar(),
            Updater = new Updater
            {
                BetaString = "-beta",
                DevString = "-dev",
                UpdateAsset = "EmbyStat-win10-x64-v{version}.zip",
                GitHubUrl = "https://api.github.com/repos/mregni/embystat",
                Dll = "updater.dll"
            }
        },
        UserConfig = new UserConfig
        {
            Language = "en-US",
            WizardFinished = false,
            ToShortMovieEnabled = true,
            ToShortMovie = 10,
            KeepLogsCount = 10,
            LogLevel = 0,
            EnableRollbarLogging = false,
            Hosting = new Hosting
            {
                Port = 6555,
                Url = "*",
                SslEnabled = false,
                SslPort = 6556,
                SslCertPassword = string.Empty,
                SslCertPath = string.Empty
            },
            MediaServer = new MediaServerSettings
            {
                AuthorizationScheme = "MediaBrowser",
                Address = string.Empty,
                Id = string.Empty,
                Name = string.Empty,
                ApiKey = string.Empty,
                UserId = string.Empty
            },

            Tmdb = new TmdbSettings
            {
                ApiKey = "0ad9610e613fdbf0d62e71c96d903e0c",
                LastUpdate = null
            }
        }
    };
}