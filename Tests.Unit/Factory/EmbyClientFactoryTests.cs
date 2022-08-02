using System;
using EmbyStat.Clients.Emby;
using EmbyStat.Clients.Emby.Http;
using EmbyStat.Clients.Jellyfin;
using EmbyStat.Clients.Jellyfin.Http;
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
        var factory = new EmbyClientFactory(baseHttpClientMock.Object);

        var client = factory.CreateHttpClient();
        client.Should().NotBeNull();
    }
    
    [Fact]
    public void CreateWebSocketClient_Should_Return_Client()
    {
        var baseHttpClientMock = new Mock<IEmbyBaseHttpClient>();
        var factory = new EmbyClientFactory(baseHttpClientMock.Object);

        var act = () => factory.CreateWebSocketClient();
        act.Should().Throw<NotImplementedException>();
    }
    
    [Theory]
    [InlineData(ServerType.Emby, true)]
    [InlineData(ServerType.Jellyfin, false)]
    public void AppliesTo_Should_Return_Correct_Value(ServerType serverType, bool result)
    {
        var baseHttpClientMock = new Mock<IEmbyBaseHttpClient>();
        var factory = new EmbyClientFactory(baseHttpClientMock.Object);

        var client = factory.AppliesTo(serverType);
        client.Should().Be(result);
    }
}