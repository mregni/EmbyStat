using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NLog;
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
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(Path.Combine(_appSettings.Dirs.Settings, "nlog.config"));
                var navigator = doc.CreateNavigator();

                var manager = new XmlNamespaceManager(navigator.NameTable);
                manager.AddNamespace("nlog", "http://www.nlog-project.org/schemas/NLog.xsd");

                foreach (XPathNavigator nav in navigator.Select("//nlog:logger[@writeTo='rollbar']", manager))
                {
                    if (nav.MoveToAttribute("enabled", ""))
                    {
                        nav.SetValue(userSettings.EnableRollbarLogging.ToString().ToLower());
                    }
                }

                doc.Save(Path.Combine(_appSettings.Dirs.Settings, "nlog.config"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


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
