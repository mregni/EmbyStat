using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NLog;
using Rollbar;
using Formatting = Newtonsoft.Json.Formatting;

namespace EmbyStat.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly AppSettings _appSettings;
        private readonly Logger _logger;
        private UserSettings _userSettings;
        public event EventHandler<GenericEventArgs<UserSettings>> OnUserSettingsChanged;

        public SettingsService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            _logger = LogManager.GetCurrentClassLogger();
            LoadUserSettingsFromFile();
            CreateRollbarLogger();
        }

        public AppSettings GetAppSettings()
        {
            return _appSettings;
        }

        public UserSettings GetUserSettings()
        {
            return _userSettings;
        }

        public async Task<UserSettings> SaveUserSettings(UserSettings userSettings)
        {
            _userSettings = userSettings;

            var strJson = JsonConvert.SerializeObject(userSettings, Formatting.Indented);
            var dir = Path.Combine(_appSettings.Dirs.Settings, "usersettings.json");
            await File.WriteAllTextAsync(dir, strJson);

            CreateRollbarLogger();

            OnUserSettingsChanged?.Invoke(this, new GenericEventArgs<UserSettings>(_userSettings));
            return _userSettings;
        }

        private void CreateRollbarLogger()
        {
            RollbarLocator.RollbarInstance.Configure(new RollbarConfig(_appSettings.Rollbar.AccessToken)
            {
                Environment = _appSettings.Rollbar.Environment,
                MaxReportsPerMinute = _appSettings.Rollbar.MaxReportsPerMinute,
                ReportingQueueDepth = _appSettings.Rollbar.ReportingQueueDepth,
                Enabled = _userSettings.EnableRollbarLogging,
                Transform = payload =>
                {
                    payload.Data.CodeVersion = _appSettings.Version;
                    payload.Data.Custom = new Dictionary<string, object>()
                    {
                        { "Framework", RuntimeInformation.FrameworkDescription },
                        { "OS", RuntimeInformation.OSDescription}
                    };
                }

            });
        }

        public async Task SetUpdateInProgressSetting(bool value)
        {
            _userSettings.UpdateInProgress = value;
            await SaveUserSettings(_userSettings);
        }

        private void LoadUserSettingsFromFile()
        {
            var dir = Path.Combine(_appSettings.Dirs.Settings, "usersettings.json");
            if (!File.Exists(dir))
            {
                var settings = new UserSettings
                {
                    AppName = "EmbyStat",
                    AutoUpdate = true,
                    KeepLogsCount = 10,
                    Language = "en-US",
                    MovieCollectionTypes = new List<CollectionType> { CollectionType.Other, CollectionType.Movies, CollectionType.HomeVideos },
                    ShowCollectionTypes = new List<CollectionType> { CollectionType.Other, CollectionType.TvShow },
                    ToShortMovie = 10,
                    UpdateInProgress = false,
                    UpdateTrain = UpdateTrain.Beta,
                    Username = string.Empty,
                    WizardFinished = false,
                    Emby = new EmbySettings(),
                    Tvdb = new TvdbSettings(),
                    EnableRollbarLogging = false
                };

                var strJson = JsonConvert.SerializeObject(settings, Formatting.Indented);
                File.WriteAllText(dir, strJson);
            }

            _userSettings = JsonConvert.DeserializeObject<UserSettings>(File.ReadAllText(dir));
            OnUserSettingsChanged?.Invoke(this, new GenericEventArgs<UserSettings>(_userSettings));
        }
    }
}
