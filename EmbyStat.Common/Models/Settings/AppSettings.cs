using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Primitives;

namespace EmbyStat.Common.Models.Settings
{
    public class AppSettings
    {
        public string Version { get; set; }
        public Updater Updater { get; set; }
        public LoggingAppSettings Logging { get; set; }
        public string ConnectionString { get; set; }
    }

    public class LoggingAppSettings
    {
        public string Directory { get; set; }
    }

    public class Updater
    {
        public string UpdateAsset { get; set; }
        public string Dll { get; set; }
        public string GithubUrl { get; set; }
        public string DevString { get; set; }
        public string BetaString { get; set; }
    }
}
