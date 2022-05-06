using EmbyStat.Services.Interfaces;
using EmbyStat.Services.Models.About;
using EmbyStat.Common.Models.Settings;
using Microsoft.Extensions.Options;
using RuntimeEnvironment = Microsoft.DotNet.PlatformAbstractions.RuntimeEnvironment;

namespace EmbyStat.Services;

public class AboutService : IAboutService
{
    public About GetAbout()
    {
        var operatingSystem = RuntimeEnvironment.OperatingSystem;
        var operatingSystemVersion = RuntimeEnvironment.OperatingSystemVersion;
        var architecture = RuntimeEnvironment.RuntimeArchitecture;

        return new About { 
            OperatingSystem = operatingSystem,
            OperatingSystemVersion = operatingSystemVersion,
            Architecture = architecture
        };
    }
}