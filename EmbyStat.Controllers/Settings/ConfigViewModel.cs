using System;

namespace EmbyStat.Controllers.Settings;

public class ConfigViewModel
{
    public UserConfigViewModel UserConfig { get; set; }
    public SystemConfigViewModel SystemConfig { get; set; }
}

public class UserConfigViewModel
{
    public string Language { get; set; }
    public bool ToShortMovieEnabled { get; set; }
    public int ToShortMovie { get; set; }
    public int KeepLogsCount { get; set; }
    public int LogLevel { get; set; }
    public bool EnableRollbarLogging { get; set; }
    public HostingViewModel Hosting { get; set; }
    public MediaServerSettingsViewModel MediaServer { get; set; }
    public TmdbSettingsViewModel Tmdb { get; set; }
}

public class SystemConfigViewModel
{
    public bool AutoUpdate { get; set; }
    public bool WizardFinished { get; set; }
    public string Version { get; set; }
    public string ProcessName { get; set; }
    public string AppName { get; set; }
    public Guid? Id { get; set; }
    public bool UpdateInProgress { get; set; }
    public bool UpdatesDisabled { get; set; }
    public long Migration { get; set; }
    public int UpdateTrain { get; set; }
    public RollbarViewModel Rollbar { get; set; }
    public DirsViewModel Dirs { get; set; }
}

public class HostingViewModel
{
    public bool SslEnabled { get; set; }
    public int Port { get; set; }
    public int SslPort { get; set; }
    public string Url { get; set; }
    public string SslCertPath { get; set; }
    public string SslCertPassword { get; set; }
}

public class MediaServerSettingsViewModel
{
    public string Name { get; set; }
    public string ApiKey { get; set; }
    public string Address { get; set; }
    public string AuthorizationScheme { get; set; }
    public int Type { get; set; }
    public string UserId { get; set; }
    public string Id { get; set; }
    public string FullSocketAddress => (Address ?? string.Empty)
        .Replace("https://", "wss://")
        .Replace("http://", "ws://");
}

public class TmdbSettingsViewModel
{
    public string ApiKey { get; set; }
}

public class RollbarViewModel
{
    public bool Enabled { get; set; }
}

public class DirsViewModel
{
    public string TempUpdate { get; set; }
    public string Updater { get; set; }
    public string Logs { get; set; }
    public string Data { get; set; }
}
