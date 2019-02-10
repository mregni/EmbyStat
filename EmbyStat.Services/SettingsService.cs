using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            LoadUserSettingsFromFile();
        }

        public AppSettings GetAppSettings()
        {
            return _appSettings;
        }

        public UserSettings GetUserSettings()
        {
            return _userSettings;
        }

        public async Task SaveUserSettingsToFile(UserSettings userSettings)
        {
            _userSettings = userSettings;
            var strJson = JsonConvert.SerializeObject(userSettings, Formatting.Indented);
            var dir = Path.Combine(_appSettings.Dirs.Settings, "usersettings.json");
            await File.WriteAllTextAsync(dir, strJson);
            OnUserSettingsChanged?.Invoke(this, new GenericEventArgs<UserSettings>(_userSettings));
        }

        public async Task SetUpdateInProgressSetting(bool value)
        {
            _userSettings.UpdateInProgress = value;
            await SaveUserSettingsToFile(_userSettings);
        }

        private void LoadUserSettingsFromFile()
        {
            var dir = Path.Combine(_appSettings.Dirs.Settings, "usersettings.json");
            _userSettings = JsonConvert.DeserializeObject<UserSettings>(File.ReadAllText(dir));
            OnUserSettingsChanged?.Invoke(this, new GenericEventArgs<UserSettings>(_userSettings));
        }
    }
}
