using EmbyStat.Core.About.Interfaces;
using Microsoft.DotNet.PlatformAbstractions;

namespace EmbyStat.Core.About;

public class AboutService : IAboutService
{
    public AboutModel GetAbout()
    {
        var operatingSystem = RuntimeEnvironment.OperatingSystem;
        var operatingSystemVersion = RuntimeEnvironment.OperatingSystemVersion;
        var architecture = RuntimeEnvironment.RuntimeArchitecture;

        return new AboutModel { 
            OperatingSystem = operatingSystem,
            OperatingSystemVersion = operatingSystemVersion,
            Architecture = architecture
        };
    }
}