using EmbyStat.Clients.Emby;
using EmbyStat.Clients.Emby.Http;
using EmbyStat.Clients.Emby.WebSocket;
using EmbyStat.Common.Enums;
using FluentAssertions;
using Moq;
using Xunit;

namespace Tests.Unit.Factory;

public class EmbyClientFactoryTests
{
    [Fact]
    public void CreateHttpClient_Should_Return_Client()
    {
        var baseHttpClientMock = new Mock<IEmbyBaseHttpClient>();
        var baseSocketClientMock = new Mock<IEmbyWebSocketHandler>();
        var factory = new EmbyClientFactory(baseHttpClientMock.Object, baseSocketClientMock.Object);

        var client = factory.CreateHttpClient();
        client.Should().NotBeNull();
    }
    
    [Fact]
    public void CreateWebSocketClient_Should_Return_Client()
    {
        var baseHttpClientMock = new Mock<IEmbyBaseHttpClient>();
        var baseSocketClientMock = new Mock<IEmbyWebSocketHandler>();
        var factory = new EmbyClientFactory(baseHttpClientMock.Object, baseSocketClientMock.Object);

        var client = factory.CreateWebSocketClient();
        client.Should().NotBeNull();
    }
    
    [Theory]
    [InlineData(ServerType.Emby, true)]
    [InlineData(ServerType.Jellyfin, false)]
    public void AppliesTo_Should_Return_Correct_Value(ServerType serverType, bool result)
    {
        var baseHttpClientMock = new Mock<IEmbyBaseHttpClient>();
        var baseSocketClientMock = new Mock<IEmbyWebSocketHandler>();
        var factory = new EmbyClientFactory(baseHttpClientMock.Object, baseSocketClientMock.Object);

        var client = factory.AppliesTo(serverType);
        client.Should().Be(result);
    }
}