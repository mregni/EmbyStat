using System;
using System.Collections.Generic;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using Newtonsoft.Json;

namespace EmbyStat.Common.Models.Settings
{
    public class UserSettings
    {
        public string AppName { get; set; }
        public Guid? Id { get; set; }
        public bool WizardFinished { get; set; }
        public string Username { get; set; }
        public string Language { get; set; }
        public int ToShortMovie { get; set; }
        public int KeepLogsCount { get; set; }
        public List<CollectionType> MovieCollectionTypes { get; set; }
        public List<CollectionType> ShowCollectionTypes { get; set; }
        public bool AutoUpdate { get; set; }
        public UpdateTrain UpdateTrain { get; set; }
        public bool UpdateInProgress { get; set; }
        public EmbySettings Emby { get; set; }
        public TvdbSettings Tvdb { get; set; }
        public bool EnableRollbarLogging { get; set; }

        [JsonIgnore]
        public string FullEmbyServerAddress
        {
            get
            {
                var protocol = Emby.ServerProtocol == ConnectionProtocol.Http ? "http" : "https";
                return $"{protocol}://{Emby.ServerAddress}:{Emby.ServerPort}";
            }
        }
    }

    public class EmbySettings
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string ServerName { get; set; }
        public string AccessToken { get; set; }
        public string ServerAddress { get; set; }
        public int ServerPort { get; set; }
        public string AuthorizationScheme { get; set; }
        public ConnectionProtocol ServerProtocol { get; set; }
    }

    public class TvdbSettings
    {
        public DateTime? LastUpdate { get; set; }
        public string ApiKey { get; set; }
    }
}
