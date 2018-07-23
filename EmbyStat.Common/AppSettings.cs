using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Primitives;

namespace EmbyStat.Common
{
    public class AppSettings
    {
        public string Version { get; set; }
        public LoggingAppSettings Logging { get; set; }
    }

    public class LoggingAppSettings
    {
        public string Directory { get; set; }
    }
}
