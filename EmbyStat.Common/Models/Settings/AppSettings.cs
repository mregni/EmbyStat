using System;
using Newtonsoft.Json;

namespace EmbyStat.Common.Models.Settings
{
    public class AppSettings
    {
        public string Version { get; set; }
        public string ProcessName { get; set; }
        public Updater Updater { get; set; }
        public Dirs Dirs { get; set; }
        public Rollbar Rollbar { get; set; }
        public string DatabaseFile { get; set; }
        /// <summary>
        /// Port number. Set dynamically when server is starting
        /// </summary>
        [JsonIgnore]
        public int Port { get; set; }
        /// <summary>
        /// If true, update flow is disabled. Set dynamically when server is starting
        /// </summary>
        [JsonIgnore]
        public bool NoUpdates { get; set; }
        /// <summary>
        /// Listeing urls. Set dynamically when server is starting
        /// </summary>
        [JsonIgnore]
        public string ListeningUrls { get; set; }
        public Jwt Jwt { get; set; }
    }

    public class Updater
    {
        public string UpdateAsset { get; set; }
        public string Dll { get; set; }
        public string GitHubUrl { get; set; }
        public string DevString { get; set; }
        public string BetaString { get; set; }
    }

    public class Dirs
    {
        public string TempUpdate { get; set; }
        public string Updater { get; set; }
        /// <summary>
        /// Log folder. Never saved, Set dynamically when server is starting.
        /// </summary>
        [JsonIgnore]
        public string Logs { get; set; }
        /// <summary>
        /// Config folder. Never saved, Set dynamically when server is starting.
        /// </summary>
        [JsonIgnore]
        public string Config { get; set; }
        /// <summary>
        /// Database folder. Never saved, Set dynamically when server is starting.
        /// </summary>
        [JsonIgnore]
        public string Data { get; set; }
    }

    public class Rollbar
    {
        public string AccessToken { get; set; }
        public string Environment { get; set; }
        public string LogLevel { get; set; }
        public int MaxReportsPerMinute { get; set; }
        public int ReportingQueueDepth { get; set; }
    }

    public class Jwt
    {
        public string Key { get; set; }
        public int AccessExpireMinutes { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public DateTime IssuedAt => DateTime.UtcNow;
        public DateTime NotBefore => DateTime.UtcNow;
        public DateTime Expiration => IssuedAt.Add(TimeSpan.FromMinutes(AccessExpireMinutes));
    }
}
