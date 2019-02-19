using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using Dapper;
using EmbyStat.Common;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Settings;
using FluentMigrator;
using Newtonsoft.Json;

namespace EmbyStat.Repositories.Migrations
{
    [Migration(5)]
    public class DeleteConfigurationTable : Migration
    {
        public override void Up()
        {
            var dbFilePath = "./data.db";
            var dbConnection = new SQLiteConnection($"Data Source={dbFilePath};");
            dbConnection.Open();

            var autoUpdate = dbConnection.Query<string>("SELECT Value FROM Configuration WHERE Id = 'AUTOUPDATE'").Single();
            var keepLogsCount = dbConnection.Query<string>("SELECT Value FROM Configuration WHERE Id = 'KEEPLOGSCOUNT'").Single();
            var language = dbConnection.Query<string>("SELECT Value FROM Configuration WHERE Id = 'LANGUAGE'").Single();
            var movieCollectionTypes = dbConnection.Query<string>("SELECT Value FROM Configuration WHERE Id = 'MOVIECOLLECTIONTYPES'").Single();
            var showCollectionTypes = dbConnection.Query<string>("SELECT Value FROM Configuration WHERE Id = 'SHOWCOLLECTIONTYPES'").Single();
            var toShortMovie = dbConnection.Query<string>("SELECT Value FROM Configuration WHERE Id = 'TOSHORTMOVIE'").Single();
            var updateInProgress = dbConnection.Query<string>("SELECT Value FROM Configuration WHERE Id = 'UPDATEINPROGRESS'").Single();
            var updateTrain = dbConnection.Query<string>("SELECT Value FROM Configuration WHERE Id = 'UPDATETRAIN'").Single();
            var userName = dbConnection.Query<string>("SELECT Value FROM Configuration WHERE Id = 'USERNAME'").Single();
            var wizardFinished = dbConnection.Query<string>("SELECT Value FROM Configuration WHERE Id = 'WIZARDFINISHED'").Single();
            var embyUserId = dbConnection.Query<string>("SELECT Value FROM Configuration WHERE Id = 'EMBYUSERID'").Single();
            var embyAccessToken = dbConnection.Query<string>("SELECT Value FROM Configuration WHERE Id = 'ACCESSTOKEN'").Single();
            var embyServerName = dbConnection.Query<string>("SELECT Value FROM Configuration WHERE Id = 'SERVERNAME'").Single();
            var embyServerAddress = dbConnection.Query<string>("SELECT Value FROM Configuration WHERE Id = 'EMBYSERVERADDRESS'").Single();
            var embyServerPort = dbConnection.Query<string>("SELECT Value FROM Configuration WHERE Id = 'EMBYSERVERPORT'").Single();
            var embyServerProtocol = dbConnection.Query<string>("SELECT Value FROM Configuration WHERE Id = 'EMBYSERVERPROTOCOL'").Single();
            var embyUserName = dbConnection.Query<string>("SELECT Value FROM Configuration WHERE Id = 'EMBYUSERNAME'").Single();
            var tvdbApiKey = dbConnection.Query<string>("SELECT Value FROM Configuration WHERE Id = 'TVDBAPICLIENT'").Single();
            var tvdbLastUpdate = dbConnection.Query<string>("SELECT Value FROM Configuration WHERE Id = 'LASTTVDBUPDATE'").Single();

            dbConnection.Close();

            var settings = new UserSettings
            {
                AppName = "EmbyStat",
                AutoUpdate = Convert.ToBoolean(autoUpdate),
                KeepLogsCount = Convert.ToInt32(keepLogsCount),
                Language = language,
                MovieCollectionTypes = JsonConvert.DeserializeObject<List<CollectionType>>(movieCollectionTypes),
                ShowCollectionTypes = JsonConvert.DeserializeObject<List<CollectionType>>(showCollectionTypes),
                ToShortMovie = Convert.ToInt32(toShortMovie),
                UpdateInProgress = Convert.ToBoolean(updateInProgress),
                UpdateTrain = (UpdateTrain)Convert.ToInt32(updateTrain),
                Username = userName,
                WizardFinished = Convert.ToBoolean(wizardFinished),
                Emby = new EmbySettings
                {
                    UserId = embyUserId,
                    AccessToken = embyAccessToken,
                    ServerName = embyServerName,
                    AuthorizationScheme = "MediaBrowser",
                    ServerAddress = embyServerAddress,
                    ServerPort = Convert.ToInt32(embyServerPort),
                    ServerProtocol = (ConnectionProtocol)Convert.ToInt32(embyServerProtocol),
                    UserName = embyUserName
                },
                Tvdb = new TvdbSettings
                {
                    ApiKey = tvdbApiKey,
                    LastUpdate = string.IsNullOrWhiteSpace(tvdbLastUpdate) ? (DateTime?)null : Convert.ToDateTime(tvdbLastUpdate)
                }
            };
            var dir = Path.Combine("Settings", "usersettings.json");
            if (File.Exists(dir))
            {
                var userSettings = JsonConvert.DeserializeObject<UserSettings>(File.ReadAllText(dir));
                settings.Id = userSettings.Id;
            }

            var strJson = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(dir, strJson);

            Delete.Table("Configuration");
        }

        public override void Down()
        {
            Create.Table("Configuration")
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey("PK_Configuration")
                .WithColumn("Value").AsString().Nullable();
        }
    }
}
