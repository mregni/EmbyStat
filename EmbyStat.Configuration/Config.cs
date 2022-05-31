using EmbyStat.Common.Enums;
using Newtonsoft.Json;

namespace EmbyStat.Configuration;

public class Config
{
    public UserConfig UserConfig { get; set; }
    public SystemConfig SystemConfig { get; set; }
}

public class UserConfig
{
    public string Language { get; set; }
    public bool ToShortMovieEnabled { get; set; }
    public int ToShortMovie { get; set; }
    public int KeepLogsCount { get; set; }
    public int LogLevel { get; set; }
    public bool EnableRollbarLogging { get; set; }
    public bool WizardFinished { get; set; }
    public Hosting Hosting { get; set; }
    public MediaServerSettings MediaServer { get; set; }
    public TmdbSettings Tmdb { get; set; }
}

public class SystemConfig
{
    public SystemConfig()
    {
        Rollbar = new Rollbar();
    }
    public bool AutoUpdate { get; set; }
    [JsonIgnore] public string Version => "0.0.0.0";
    public string ProcessName { get; set; }
    public string AppName { get; set; }
    public Guid? Id { get; set; }
    public bool UpdateInProgress { get; set; }
    public bool UpdatesDisabled { get; set; }
    public long Migration { get; set; }
    public UpdateTrain UpdateTrain { get; set; }
    public Updater Updater { get; set; }
    public Jwt Jwt { get; set; }
    [JsonIgnore]
    public Rollbar Rollbar { get; set; }
    public Dirs Dirs { get; set; }
}

public class Hosting
{
    public bool SslEnabled { get; set; }
    public int Port { get; set; }
    public int SslPort { get; set; }
    public string Url { get; set; }
    public string SslCertPath { get; set; }
    public string SslCertPassword { get; set; }
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
    public string FullSocketAddress => (Address ?? string.Empty)
        .Replace("https://", "wss://")
        .Replace("http://", "ws://");
}

public class Updater
{
    public string UpdateAsset { get; set; }
    public string GitHubUrl { get; set; }
    public string DevString { get; set; }
    public string BetaString { get; set; }
    public string Dll { get; set; }
}

public class TmdbSettings
{
    public DateTime? LastUpdate { get; set; }
    public string ApiKey { get; set; }
}

public class Rollbar
{
    public string AccessToken => "RollbarAccessToken";
    public string Environment => "RollbarEnvironment";
}

public class Dirs
{
    public string TempUpdate { get; set; }
    public string Updater { get; set; }
    public string Logs { get; set; }
    public string Data { get; set; }
}

public class Jwt
{
    public string Key { get; set; }
    public int AccessExpireMinutes { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    [JsonIgnore] public DateTime IssuedAt => DateTime.UtcNow;
    [JsonIgnore] public DateTime NotBefore => DateTime.UtcNow;
    [JsonIgnore] public DateTime Expiration => IssuedAt.Add(TimeSpan.FromMinutes(AccessExpireMinutes));
}