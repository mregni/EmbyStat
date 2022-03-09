﻿using EmbyStat.Common.Enums;

namespace EmbyStat.Common.SqLite;

public class SqlServerInfo
{
    public string Id { get; set; }
    public string SystemUpdateLevel { get; set; }
    public string OperatingSystemDisplayName { get; set; }
    public bool HasPendingRestart { get; set; }
    public bool SupportsLibraryMonitor { get; set; }
    public int WebSocketPortNumber { get; set; }
    public bool CanSelfRestart { get; set; }
    public bool CanSelfUpdate { get; set; }
    public bool CanLaunchWebBrowser { get; set; }
    public string ProgramDataPath { get; set; }
    public string ItemsByNamePath { get; set; }
    public string CachePath { get; set; }
    public string LogPath { get; set; }
    public string InternalMetadataPath { get; set; }
    public string TranscodingTempPath { get; set; }
    public int HttpServerPortNumber { get; set; }
    public bool SupportsHttps { get; set; }
    public int HttpsPortNumber { get; set; }
    public bool HasUpdateAvailable { get; set; }
    public bool SupportsAutoRunAtStartup { get; set; }
    public bool HardwareAccelerationRequiresPremiere { get; set; }
    public string LocalAddress { get; set; }
    public string WanAddress { get; set; }
    public string ServerName { get; set; }
    public string Version { get; set; }
    public string OperatingSystem { get; set; }
}