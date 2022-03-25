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
        public bool AutoUpdate { get; set; }
        public UpdateTrain UpdateTrain { get; set; }
        public bool UpdateInProgress { get; set; }
        public MediaServerSettings MediaServer { get; set; }
        public TmdbSettings Tmdb { get; set; }
        public bool EnableRollbarLogging { get; set; }
    }

    public class MediaServerSettings
    {
        public string Name { get; set; }
        public string ApiKey { get; set; }
        public string Address { get; set; }
        public string AuthorizationScheme { get; set; }
        public ServerType Type { get; set; }
        public string UserId { get; set; }
        public string Id { get; set; }

        [JsonIgnore]
        public string FullSocketAddress => Address
            .Replace("https://", "wss://")
            .Replace("http://", "ws://");
    }

    public class TmdbSettings
    {
        public DateTime? LastUpdate { get; set; }
        public string ApiKey { get; set; }
    }

    public class LibraryContainer
    {
        public string Id { get; set; }
        public DateTime? LastSynced { get; set; }
        public string Name { get; set; }
    }
}
