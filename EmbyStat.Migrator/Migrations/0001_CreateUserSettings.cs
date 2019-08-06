using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Migrator.Models;
using EmbyStat.Services.Interfaces;
using Newtonsoft.Json;

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
                    MovieCollectionTypes = new List<CollectionType> { CollectionType.Other, CollectionType.Movies, CollectionType.HomeVideos },
                    ShowCollectionTypes = new List<CollectionType> { CollectionType.Other, CollectionType.TvShow },
                    ToShortMovieEnabled = true,
                    ToShortMovie = 10,
                    UpdateInProgress = false,
                    UpdateTrain = UpdateTrain.Beta,
                    Username = string.Empty,
                    Version = 0,
                    WizardFinished = false,
                    Emby = new EmbySettings
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
