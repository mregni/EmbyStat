using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Services;
using FluentAssertions;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Tests.Unit.Services
{
	[Collection("Mapper collection")]
    public class AboutServiceTests
    {
        private readonly Mock<IOptions<AppSettings>> _appSettingsMock;
        private readonly AboutService _aboutService;

        public AboutServiceTests()
        {
            var appSettings = new AppSettings
            {
                Version = "0.0.1.0"
            };

            _appSettingsMock = new Mock<IOptions<AppSettings>>();
            _appSettingsMock.Setup(x => x.Value).Returns(appSettings);

            _aboutService = new AboutService(_appSettingsMock.Object);
        }

        [Fact]
        public void GetAboutInfo()
        {
            var about = _aboutService.GetAbout();

            about.Should().NotBeNull();
            about.Version.Should().Be("0.0.1.0");
            about.Architecture.Should().Be(RuntimeEnvironment.RuntimeArchitecture);
            about.OperatingSystem.Should().Be(RuntimeEnvironment.OperatingSystem);
            about.OperatingSystemVersion.Should().Be(RuntimeEnvironment.OperatingSystemVersion);

            _appSettingsMock.VerifyGet(x => x.Value, Times.Exactly(1));
        }
    }
}
