using System;
using System.Runtime.InteropServices;
using EmbyStat.Services.Interfaces;
using EmbyStat.Services.Models.About;
using EmbyStat.Common.Models.Settings;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.Options;
using RuntimeEnvironment = Microsoft.DotNet.PlatformAbstractions.RuntimeEnvironment;

namespace EmbyStat.Services
{
    public class AboutService : IAboutService
    {
        private readonly IOptions<AppSettings> _options;

        public AboutService(IOptions<AppSettings> options)
        {
            _options = options;
        }

        public About GetAbout()
        {
            var version = _options.Value.Version;
            var operatingSystem = RuntimeEnvironment.OperatingSystem;
            var operatingSystemVersion = RuntimeEnvironment.OperatingSystemVersion;
            var architecture = RuntimeEnvironment.RuntimeArchitecture;

            return new About { 
                Version = version,
                OperatingSystem = operatingSystem,
                OperatingSystemVersion = operatingSystemVersion,
                Architecture = architecture
            };
        }
    }
}
