using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Api.Github.Models
{
    public class CheckForUpdateResult
    {
        public CheckForUpdateResult()
        {
            IsUpdateAvailable = false;
        }
        public bool IsUpdateAvailable { get; set; }
        public string AvailableVersion { get; set; }
        public PackageInfo Package { get; set; }
    }
}
