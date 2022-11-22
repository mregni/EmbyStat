using System;
using EmbyStat.Clients.Jellyfin;
using EmbyStat.Clients.Jellyfin.Http;
using EmbyStat.Clients.Jellyfin.WebSocket;
using EmbyStat.Common.Enums;
using FluentAssertions;
using Moq;
using Xunit;

namespace Tests.Unit.Factory;

public class JellyfinClientFactoryTests
{
    [Fact]
    public void CreateHttpClient_Should_Return_Client()
    {
        var baseHttpClientMock = new Mock<IJellyfinBaseHttpClient>();
        var baseSocketClientMock = new Mock<IJellyfinWebSocketHandler>();

        var factory = new JellyfinClientFactory(baseHttpClientMock.Object, baseSocketClientMock.Object);

        var client = factory.CreateHttpClient();
        client.Should().NotBeNull();
    }
    
    [Fact]
    public void CreateWebSocketClient_Should_Return_Client()
    {
        var baseHttpClientMock = new Mock<IJellyfinBaseHttpClient>();
        var baseSocketClientMock = new Mock<IJellyfinWebSocketHandler>();

        var factory = new JellyfinClientFactory(baseHttpClientMock.Object, baseSocketClientMock.Object);

        var client = factory.CreateWebSocketClient();
        client.Should().NotBeNull();
    }
    
    [Theory]
    [InlineData(ServerType.Emby, false)]
    [InlineData(ServerType.Jellyfin, true)]
    public void AppliesTo_Should_Return_Correct_Value(ServerType serverType, bool result)
    {
        var baseHttpClientMock = new Mock<IJellyfinBaseHttpClient>();
        var baseSocketClientMock = new Mock<IJellyfinWebSocketHandler>();

        var factory = new JellyfinClientFactory(baseHttpClientMock.Object, baseSocketClientMock.Object);

        var client = factory.AppliesTo(serverType);
        client.Should().Be(result);
    }
}