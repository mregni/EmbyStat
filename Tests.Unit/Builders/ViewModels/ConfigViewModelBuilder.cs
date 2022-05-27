using EmbyStat.Configuration;
using EmbyStat.Controllers.Settings;
using FluentAssertions;

namespace Tests.Unit.Builders.ViewModels;

public class ConfigViewModelBuilder
{
    private readonly ConfigViewModel _model;

    public ConfigViewModelBuilder(Config config)
    {
        _model = new ConfigViewModel
        {
            SystemConfig = new SystemConfigViewModel
            {
                AutoUpdate = config.SystemConfig.AutoUpdate,
                AppName = config.SystemConfig.AppName,
                Version = config.SystemConfig.Version,
                ProcessName = config.SystemConfig.ProcessName,
                Id = config.SystemConfig.Id,
                UpdateInProgress = config.SystemConfig.UpdateInProgress,
                UpdatesDisabled = config.SystemConfig.UpdatesDisabled,
                Migration = config.SystemConfig.Migration,
                UpdateTrain = (int)config.SystemConfig.UpdateTrain,
                Dirs = new DirsViewModel
                {
                    Data = config.SystemConfig.Dirs.Data,
                    Logs = config.SystemConfig.Dirs.Logs,
                    Updater = config.SystemConfig.Dirs.Updater,
                    TempUpdate = config.SystemConfig.Dirs.TempUpdate
                },
                Rollbar = new RollbarViewModel()
            },
            UserConfig = new UserConfigViewModel()
            {
                Hosting = new HostingViewModel()
                {
                    Port = config.UserConfig.Hosting.Port,
                    Url = config.UserConfig.Hosting.Url,
                    SslEnabled = config.UserConfig.Hosting.SslEnabled,
                    SslPort = config.UserConfig.Hosting.SslPort,
                    SslCertPassword = config.UserConfig.Hosting.SslCertPassword,
                    SslCertPath = config.UserConfig.Hosting.SslCertPath
                },
                Language = config.UserConfig.Language,
                Tmdb = new TmdbSettingsViewModel()
                {
                    ApiKey = config.UserConfig.Tmdb.ApiKey
                },
                LogLevel = config.UserConfig.LogLevel,
                MediaServer = new MediaServerSettingsViewModel()
                {
                    Address = config.UserConfig.MediaServer.Address,
                    Id = config.UserConfig.MediaServer.Id,
                    Name = config.UserConfig.MediaServer.Name,
                    Type = (int)config.UserConfig.MediaServer.Type,
                    ApiKey = config.UserConfig.MediaServer.ApiKey,
                    AuthorizationScheme = config.UserConfig.MediaServer.AuthorizationScheme,
                    UserId = config.UserConfig.MediaServer.UserId,
                    FullSocketAddress = config.UserConfig.MediaServer.FullSocketAddress
                },
                WizardFinished = config.UserConfig.WizardFinished,
                EnableRollbarLogging = config.UserConfig.EnableRollbarLogging,
                KeepLogsCount = config.UserConfig.KeepLogsCount,
                ToShortMovie = config.UserConfig.ToShortMovie,
                ToShortMovieEnabled = config.UserConfig.ToShortMovieEnabled
            }
        };
    }

    public ConfigViewModel Build()
    {
        return _model;
    }
}