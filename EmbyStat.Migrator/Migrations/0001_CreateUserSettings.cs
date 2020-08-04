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
                    MovieLibraries = new List<string>(),
                    ShowLibraries = new List<string>(),
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
                    Tvdb = new TvdbSettings
                    {
                        ApiKey = "BWLRSNRC0AQUIEYX"
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
