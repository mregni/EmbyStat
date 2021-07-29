using System;
using System.Collections.Generic;
using EmbyStat.Common.Enums;
using Newtonsoft.Json;

namespace EmbyStat.Common.Models.Settings
{
    public class UserSettings
    {
        public string AppName { get; set; }
        public Guid? Id { get; set; }
        public long Version { get; set; }
        public bool WizardFinished { get; set; }
        public string Language { get; set; }
        public bool ToShortMovieEnabled { get; set; }
        public int ToShortMovie { get; set; }
        public int KeepLogsCount { get; set; }
        public List<string> MovieLibraries { get; set; }
        public List<string> ShowLibraries { get; set; }
        public bool AutoUpdate { get; set; }
        public UpdateTrain UpdateTrain { get; set; }
        public bool UpdateInProgress { get; set; }
        public MediaServerSettings MediaServer { get; set; }
        public TmdbSettings Tmdb { get; set; }
        public bool EnableRollbarLogging { get; set; }
    }

    public class MediaServerSettings
    {
        public string ServerName { get; set; }
        public string ApiKey { get; set; }
        public string ServerAddress { get; set; }
        public int ServerPort { get; set; }
        public string AuthorizationScheme { get; set; }
        public ConnectionProtocol ServerProtocol { get; set; }
        public ServerType ServerType { get; set; }
        public string UserId { get; set; }
        public string ServerBaseUrl { get; set; }
        public string ServerId { get; set; }

        [JsonIgnore]
        public string FullMediaServerAddress
        {
            get
            {
                var protocol = ServerProtocol == ConnectionProtocol.Https ? "https" : "http";
                var baseUrl = ServerBaseUrl == "/" ? string.Empty : ServerBaseUrl;
                return $"{protocol}://{ServerAddress}:{ServerPort}{baseUrl}";
            }
        }

        [JsonIgnore]
        public string FullSocketAddress
        {
            get
            {
                var protocol = ServerProtocol == ConnectionProtocol.Https ? "wss" : "ws";
                var baseUrl = ServerBaseUrl == "/" ? string.Empty : ServerBaseUrl;
                return $"{protocol}://{ServerAddress}:{ServerPort}{baseUrl}";
            }
        }
    }

    public class TmdbSettings
    {
        public DateTime? LastUpdate { get; set; }
        public string ApiKey { get; set; }
    }
}
