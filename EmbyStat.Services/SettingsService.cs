using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NLog;

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
            LoadUserSettingsFromFile();
            _logger = LogManager.GetCurrentClassLogger();
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
            OnUserSettingsChanged?.Invoke(this, new GenericEventArgs<UserSettings>(_userSettings));

            return _userSettings;
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
                var e = new FileNotFoundException("usersettings.json could not be found. Place it in the correct folder and restart the server (default folder: Settings)");
                _logger.Error(e, "usersettings.json file not found");
                throw e;
            }

            _userSettings = JsonConvert.DeserializeObject<UserSettings>(File.ReadAllText(dir));
            OnUserSettingsChanged?.Invoke(this, new GenericEventArgs<UserSettings>(_userSettings));
        }
    }
}
