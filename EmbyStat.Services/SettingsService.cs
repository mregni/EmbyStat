using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;

namespace EmbyStat.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly AppSettings _appSettings;
        private readonly IStatisticsRepository _statisticsRepository;
        private UserSettings _userSettings;
        public event EventHandler<GenericEventArgs<UserSettings>> OnUserSettingsChanged;

        public SettingsService(IOptions<AppSettings> appSettings, IStatisticsRepository statisticsRepository)
        {
            _appSettings = appSettings.Value;
            _statisticsRepository = statisticsRepository;
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

        public async Task<UserSettings> SaveUserSettings(UserSettings userSettings)
        {
            MarkMovieStatisticsAsInvalidIfNeeded(userSettings);
            MarkShowStatisticsAsInvalidIfNeeded(userSettings);
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
                Log.Error(e, "usersettings.json file not found");
                throw e;
            }

            _userSettings = JsonConvert.DeserializeObject<UserSettings>(File.ReadAllText(dir));
            OnUserSettingsChanged?.Invoke(this, new GenericEventArgs<UserSettings>(_userSettings));
        }

        private void MarkMovieStatisticsAsInvalidIfNeeded(UserSettings configuration)
        {
            if (!(_userSettings.MovieCollectionTypes.All(configuration.MovieCollectionTypes.Contains) &&
                  _userSettings.MovieCollectionTypes.Count == configuration.MovieCollectionTypes.Count))
            {
                _statisticsRepository.MarkMovieTypesAsInvalid();
            }
        }

        private void MarkShowStatisticsAsInvalidIfNeeded(UserSettings configuration)
        {
            if (!(_userSettings.ShowCollectionTypes.All(configuration.ShowCollectionTypes.Contains) &&
                  _userSettings.ShowCollectionTypes.Count == configuration.ShowCollectionTypes.Count))
            {
                _statisticsRepository.MarkShowTypesAsInvalid();
            }
        }
    }
}
