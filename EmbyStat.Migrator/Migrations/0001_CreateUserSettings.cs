using System.Collections.Generic;
using System.IO;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Migrator.Models;

namespace EmbyStat.Migrator.Migrations
{
    [Migration(1)]
    public class CreateUserSettings : Migration
    {
        public override void Up()
        {
            var dir = Path.Combine(AppSettings.Dirs.Config, "usersettings.json");
            if (!File.Exists(dir))
            {
                UserSettings = new UserSettings
                {
                    AppName = "EmbyStat",
                    AutoUpdate = true,
                    KeepLogsCount = 10,
                    Language = "en-US",
                    ToShortMovieEnabled = true,
                    ToShortMovie = 10,
                    UpdateInProgress = false,
                    UpdateTrain = UpdateTrain.Beta,
                    Version = 0,
                    WizardFinished = false,
                    MediaServer = new MediaServerSettings
                    {
                        AuthorizationScheme = "MediaBrowser"
                    },
                    Tmdb = new TmdbSettings
                    {
                        ApiKey = "0ad9610e613fdbf0d62e71c96d903e0c"
                    },
                    EnableRollbarLogging = false
                };
            }
            else
            {
                UserSettings = SettingsService.GetUserSettings();
            }
        }
    }
}
