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
        private UserSettings _userSettings;
        public event EventHandler<GenericEventArgs<UserSettings>> OnUserSettingsChanged;

        public SettingsService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public AppSettings GetAppSettings()
        {
            return _appSettings;
        }

        public UserSettings GetUserSettings()
        {
            return _userSettings;
        }

        public Task<UserSettings> SaveUserSettingsAsync(UserSettings userSettings)
        {
            return SaveUserSettingsAsync(userSettings, _userSettings.Version);
        }

        public async Task<UserSettings> SaveUserSettingsAsync(UserSettings userSettings, long version)
        {
            _userSettings = userSettings;
            _userSettings.Version = version;

            var strJson = JsonConvert.SerializeObject(_userSettings, Formatting.Indented);
            var dir = Path.Combine(_appSettings.Dirs.Settings, "usersettings.json");
            await File.WriteAllTextAsync(dir, strJson);

            CreateRollbarLogger();

            OnUserSettingsChanged?.Invoke(this, new GenericEventArgs<UserSettings>(_userSettings));
            return _userSettings;
        }

        public void CreateRollbarLogger()
        {
            var rollbarConfig = new RollbarConfig(_appSettings.Rollbar.AccessToken)
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
                        {"Framework", RuntimeInformation.FrameworkDescription},
                        {"OS", RuntimeInformation.OSDescription}
                    };
                }
            };

            RollbarLocator.RollbarInstance.Configure(rollbarConfig);
        }

        public long GetUserSettingsVersion()
        {
            return _userSettings?.Version ?? 0;
        }

        public Task SetUpdateInProgressSettingAsync(bool value)
        {
            _userSettings.UpdateInProgress = value;
            return SaveUserSettingsAsync(_userSettings);
        }

        public void LoadUserSettingsFromFile()
        {
            var dir = Path.Combine(_appSettings.Dirs.Settings, "usersettings.json");
            if (File.Exists(dir))
            {
                _userSettings = JsonConvert.DeserializeObject<UserSettings>(File.ReadAllText(dir));
                OnUserSettingsChanged?.Invoke(this, new GenericEventArgs<UserSettings>(_userSettings));
            }
        }
    }
}
