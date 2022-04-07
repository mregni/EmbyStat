using EmbyStat.Common.Models;
using FluentAssertions;
using Xunit;

namespace Tests.Unit.Models
{
    public class EmbyUdpBroadcastTests
    {
        [Fact]
        public void ConvertHttpIp()
        {
            const string address = "http://192.168.1.1:801";
            var broadcastModel = new MediaServerUdpBroadcast
            {
                Address = address
            };

            broadcastModel.Protocol.Should().Be(1);
            broadcastModel.Address.Should().Be("192.168.1.1");
            broadcastModel.Port.Should().Be(801);
        }

        [Fact]
        public void ConvertHttpAddress()
        {
            const string address = "http://my.domain.com:80001";
            var broadcastModel = new MediaServerUdpBroadcast
            {
                Address = address
            };

            broadcastModel.Protocol.Should().Be(1);
            broadcastModel.Address.Should().Be("my.domain.com");
            broadcastModel.Port.Should().Be(80001);
        }

        [Fact]
        public void ConvertHttpsIp()
        {
            const string address = "https://192.168.1.1:8001";
            var broadcastModel = new MediaServerUdpBroadcast
            {
                Address = address
            };

            broadcastModel.Protocol.Should().Be(0);
            broadcastModel.Address.Should().Be("192.168.1.1");
            broadcastModel.Port.Should().Be(8001);
        }

        [Fact]
        public void ConvertHttpsAddress()
        {
            const string address = "https://my.domain.com:8001";
            var broadcastModel = new MediaServerUdpBroadcast
            {
                Address = address
            };

            broadcastModel.Protocol.Should().Be(0);
            broadcastModel.Address.Should().Be("my.domain.com");
            broadcastModel.Port.Should().Be(8001);
        }
    }
}
