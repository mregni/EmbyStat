using EmbyStat.Core.About;
using FluentAssertions;
using Microsoft.DotNet.PlatformAbstractions;
using Xunit;

namespace Tests.Unit.Services;

public class AboutServiceTests
{
    private readonly AboutService _aboutService;

    public AboutServiceTests()
    {
        _aboutService = new AboutService();
    }

    [Fact]
    public void GetAboutInfo()
    {
        var about = _aboutService.GetAbout();

        about.Should().NotBeNull();
        about.Architecture.Should().Be(RuntimeEnvironment.RuntimeArchitecture);
        about.OperatingSystem.Should().Be(RuntimeEnvironment.OperatingSystem);
        about.OperatingSystemVersion.Should().Be(RuntimeEnvironment.OperatingSystemVersion);
    }
}