using System;
using EmbyStat.Clients.Jellyfin;
using EmbyStat.Clients.Jellyfin.Http;
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
        var factory = new JellyfinClientFactory(baseHttpClientMock.Object);

        var client = factory.CreateHttpClient();
        client.Should().NotBeNull();
    }
    
    [Fact]
    public void CreateWebSocketClient_Should_Return_Client()
    {
        var baseHttpClientMock = new Mock<IJellyfinBaseHttpClient>();
        var factory = new JellyfinClientFactory(baseHttpClientMock.Object);

        var act = () => factory.CreateWebSocketClient();
        act.Should().Throw<NotImplementedException>();
    }
    
    [Theory]
    [InlineData(ServerType.Emby, false)]
    [InlineData(ServerType.Jellyfin, true)]
    public void AppliesTo_Should_Return_Correct_Value(ServerType serverType, bool result)
    {
        var baseHttpClientMock = new Mock<IJellyfinBaseHttpClient>();
        var factory = new JellyfinClientFactory(baseHttpClientMock.Object);

        var client = factory.AppliesTo(serverType);
        client.Should().Be(result);
    }
}