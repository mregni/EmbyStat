using System;
using EmbyStat.Common.Enums;
using Newtonsoft.Json;

namespace EmbyStat.Common.Models.Settings;

public class AppSettings
{
    public string Version { get; set; }
    public string ProcessName { get; set; }
    public string AppName { get; set; }
    public Guid? Id { get; set; }
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
    public long Migration { get; set; }

}

public class Updater
{
    public string UpdateAsset { get; set; }
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