using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Clients.Base.Api;
using EmbyStat.Clients.Base.Http;
using EmbyStat.Clients.Emby.Http;
using EmbyStat.Clients.Jellyfin.Http;
using EmbyStat.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace Tests.Unit.Clients;

public class JellyFinHttpClientTests
{
    private readonly Mock<IMediaServerApi> _restClientMock;
    private readonly JellyfinBaseHttpClient _service;
    private readonly Mock<IRefitHttpClientFactory<IMediaServerApi>> _factoryMock;
    private readonly string _authorizationParameter;

    public JellyFinHttpClientTests()
    {
        _restClientMock = new Mock<IMediaServerApi>();
        _restClientMock
            .Setup(x => x.Ping(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("\"Jellyfin Server\"");

        _factoryMock = new Mock<IRefitHttpClientFactory<IMediaServerApi>>();
        _factoryMock
            .Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(_restClientMock.Object);

        var mappingConfig = new MapperConfiguration(mc => { mc.AddProfile(new MapProfiles()); });
        var mapper = mappingConfig.CreateMapper();

        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _service = new JellyfinBaseHttpClient(httpContextAccessorMock.Object, _factoryMock.Object, mapper);

        _service.SetDeviceInfo("embystat", "mediabrowser", "0", "c",
            "fa89fb6c-f3b7-4cc5-bc17-9522e3b94246");
        _service.BaseUrl = "localhost:9000";
        _service.ApiKey = "apikey";
        _authorizationParameter =
            "mediabrowser RestClient=\"other\", DeviceId=\"c\", Device=\"embystat\", Version=\"0\"";
    }

    [Fact]
    public async Task Ping_Should_Be_A_Success()
    {
        var result = await _service.Ping();
        result.Should().BeTrue();

        _restClientMock.Verify(x => x.Ping(_service.ApiKey, _authorizationParameter));
        _restClientMock.VerifyNoOtherCalls();

        _factoryMock.Verify(x => x.CreateClient(_service.BaseUrl));
        _factoryMock.VerifyNoOtherCalls();
    }
}