﻿using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Net;

namespace EmbyStat.Common.Converters
{
    public static class ServerInfoConverter
    {
        public static ServerInfo ConvertToInfo(this ServerInfoDto info)
        {
            if (info != null)
            {
                return new ServerInfo
                {
                    CachePath = info.CachePath,
                    CanLaunchWebBrowser = info.CanLaunchWebBrowser,
                    CanSelfRestart = info.CanSelfRestart,
                    CanSelfUpdate = info.CanSelfUpdate,
                    HardwareAccelerationRequiresPremiere = info.HardwareAccelerationRequiresPremiere,
                    HasPendingRestart = info.HasPendingRestart,
                    HasUpdateAvailable = info.HasUpdateAvailable,
                    HttpServerPortNumber = info.HttpServerPortNumber,
                    HttpsPortNumber = info.HttpsPortNumber,
                    Id = info.Id,
                    InternalMetadataPath = info.InternalMetadataPath,
                    IsShuttingDown = info.IsShuttingDown,
                    ItemsByNamePath = info.ItemsByNamePath,
                    LocalAddress = info.LocalAddress,
                    LogPath = info.LogPath,
                    OperatingSystem = info.OperatingSystem,
                    OperatingSystemDisplayName = info.OperatingSystemDisplayName,
                    ProgramDataPath = info.ProgramDataPath,
                    ServerName = info.ServerName,
                    SupportsAutoRunAtStartup = info.SupportsAutoRunAtStartup,
                    SupportsHttps = info.SupportsHttps,
                    SupportsLibraryMonitor = info.SupportsLibraryMonitor,
                    SystemUpdateLevel = MapStringToUpdateLevel(info.SystemUpdateLevel),
                    TranscodingTempPath = info.TranscodingTempPath,
                    Version = info.Version,
                    WanAddress = info.WanAddress,
                    WebSocketPortNumber = info.WebSocketPortNumber
                };
            }

            return null;
        }

        private static UpdateLevel MapStringToUpdateLevel(string level)
        {
            return level switch
            {
                "Release" => UpdateLevel.Release,
                "beta" => UpdateLevel.Beta,
                _ => UpdateLevel.Dev
            };
        }
    }
}
