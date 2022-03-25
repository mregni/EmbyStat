using EmbyStat.Common.Models.Settings;
using FluentAssertions;
using Xunit;

namespace Tests.Unit.Models
{
    public class MediaServerSettingsTests
    {
        [Fact]
        public void FullMediaServerAddress_Should_Return_HTTP_Link()
        {
            var settings = new UserSettings();
            settings.MediaServer = new MediaServerSettings
            {
                ServerBaseUrl = "",
                ServerAddress = "192.168.0.1",
                ServerPort = 8000,
                ServerProtocol = ConnectionProtocol.Http,
            };

            var address = settings.MediaServer.FullMediaServerAddress;
            address.Should().Be("http://192.168.0.1:8000");
        }

        [Fact]
        public void FullMediaServerAddress_Should_Return_HTTPS_Link()
        {
            var settings = new UserSettings();
            settings.MediaServer = new MediaServerSettings
            {
                ServerBaseUrl = "",
                ServerAddress = "192.168.0.1",
                ServerPort = 8000,
                ServerProtocol = ConnectionProtocol.Https,
            };

            var address = settings.MediaServer.FullMediaServerAddress;
            address.Should().Be("https://192.168.0.1:8000");
        }

        [Fact]
        public void FullMediaServerAddress_Should_Return_HTTP_Link_With_BaseUrl()
        {
            var settings = new UserSettings();
            settings.MediaServer = new MediaServerSettings
            {
                ServerBaseUrl = "/testing",
                ServerAddress = "192.168.0.1",
                ServerPort = 8000,
                ServerProtocol = ConnectionProtocol.Https,
            };

            var address = settings.MediaServer.FullMediaServerAddress;
            address.Should().Be("https://192.168.0.1:8000/testing");
        }

        [Fact]
        public void FullMediaServerAddress_Should_Return_HTTP_Link_Slash_BaseUrl()
        {
            var settings = new UserSettings();
            settings.MediaServer = new MediaServerSettings
            {
                ServerBaseUrl = "/",
                ServerAddress = "192.168.0.1",
                ServerPort = 8000,
                ServerProtocol = ConnectionProtocol.Https,
            };

            var address = settings.MediaServer.FullMediaServerAddress;
            address.Should().Be("https://192.168.0.1:8000");
        }

        [Fact]
        public void FullSocketAddress_Should_Return_HTTP_Link()
        {
            var settings = new UserSettings();
            settings.MediaServer = new MediaServerSettings
            {
                ServerBaseUrl = "",
                ServerAddress = "192.168.0.1",
                ServerPort = 8000,
                ServerProtocol = ConnectionProtocol.Http,
            };

            var address = settings.MediaServer.FullSocketAddress;
            address.Should().Be("ws://192.168.0.1:8000");
        }

        [Fact]
        public void FullSocketAddress_Should_Return_HTTPS_Link()
        {
            var settings = new UserSettings();
            settings.MediaServer = new MediaServerSettings
            {
                ServerBaseUrl = "",
                ServerAddress = "192.168.0.1",
                ServerPort = 8000,
                ServerProtocol = ConnectionProtocol.Https,
            };

            var address = settings.MediaServer.FullSocketAddress;
            address.Should().Be("wss://192.168.0.1:8000");
        }

        [Fact]
        public void FullSocketAddress_Should_Return_HTTP_Link_With_BaseUrl()
        {
            var settings = new UserSettings();
            settings.MediaServer = new MediaServerSettings
            {
                ServerBaseUrl = "/testing",
                ServerAddress = "192.168.0.1",
                ServerPort = 8000,
                ServerProtocol = ConnectionProtocol.Https,
            };

            var address = settings.MediaServer.FullSocketAddress;
            address.Should().Be("wss://192.168.0.1:8000/testing");
        }

        [Fact]
        public void FullSocketAddress_Should_Return_HTTP_Link_Slash_BaseUrl()
        {
            var settings = new UserSettings();
            settings.MediaServer = new MediaServerSettings
            {
                ServerBaseUrl = "/",
                ServerAddress = "192.168.0.1",
                ServerPort = 8000,
                ServerProtocol = ConnectionProtocol.Https,
            };

            var address = settings.MediaServer.FullSocketAddress;
            address.Should().Be("wss://192.168.0.1:8000");
        }
    }
}
