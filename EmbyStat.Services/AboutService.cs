using EmbyStat.Services.Interfaces;
using EmbyStat.Services.Models.About;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.DotNet.PlatformAbstractions;

namespace EmbyStat.Services
{
    public class AboutService : IAboutService
    {
        public About GetAbout()
        {
            var version = Assembly.GetEntryAssembly().GetName().Version;
            var operatingSystem = RuntimeEnvironment.OperatingSystem;
            var operatingSystemVersion = RuntimeEnvironment.OperatingSystemVersion;
            var architecture = RuntimeEnvironment.RuntimeArchitecture;

            return new About { 
                Version = version.ToString(),
                OperatingSystem = operatingSystem,
                OperatingSystemVersion = operatingSystemVersion,
                Architecture = architecture
            };
        }
    }
}
