using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Clients.Base.Api;
using EmbyStat.Clients.Base.Http;
using EmbyStat.Clients.Emby.Http;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Movies;
using EmbyStat.Common.Models.Net;
using EmbyStat.Controllers;
using FluentAssertions;
using MediaBrowser.Model.Querying;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Refit;
using Xunit;

namespace Tests.Unit.Clients;

public class BaseHttpClientTests
{
    private readonly Mock<IMediaServerApi> _restClientMock;
    private readonly EmbyBaseHttpClient _service;
    private readonly Mock<IRefitHttpClientFactory<IMediaServerApi>> _factoryMock;
    private readonly string _authorizationParameter;

    public BaseHttpClientTests()
    {
        var boe2 = new QueryResult<BaseItemDto>
        {
            Items = new[]
            {
                new BaseItemDto
                {
                    Id = "1",
                    Name = "Movies"
                }
            },
            TotalRecordCount = 1
        };
        
        _restClientMock = new Mock<IMediaServerApi>();
        _restClientMock
            .Setup(x => x.GetMediaFolders(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(boe2);
        _restClientMock
            .Setup(x => x.GetPlugins(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new List<PluginInfo> {new() {Id = "1", Name = "testPlugin"}});
        _restClientMock
            .Setup(x => x.GetServerInfo(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new ApiResponse<MediaServerInfo>(new HttpResponseMessage(HttpStatusCode.Accepted),
                new MediaServerInfo {ServerName = "server-name"}, new RefitSettings()));
        _restClientMock
            .Setup(x => x.GetUsers(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new BaseUserDto []{new() {Id = "1", Name = "Mike"}});
        _restClientMock
            .Setup(x => x.GetDevices(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new QueryResult<Device>
                {Items = new Device[] {new() {Id = "1", Name = "Web"}}, TotalRecordCount = 1});
        _restClientMock
            .Setup(x => x.GetItems(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
            .ReturnsAsync(new QueryResult<BaseItemDto>
                {Items = new[] {new BaseItemDto {Id = "1", Name = "Lord of the rings"}}, TotalRecordCount = 1});
        _restClientMock
            .Setup(x => x.GetGenres(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ItemQuery>()))
            .ReturnsAsync(new QueryResult<BaseItemDto>
                {Items = new[] {new BaseItemDto {Id = "1", Name = "Action"}}, TotalRecordCount = 1});
        _restClientMock
            .Setup(x => x.GetPeople(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ItemQuery>()))
            .ReturnsAsync(new QueryResult<BaseItemDto>
                {Items = new[] {new BaseItemDto {Id = "1", Name = "Will"}, new BaseItemDto {Id = "2", Name = "John"}}, TotalRecordCount = 2});
        _restClientMock
            .Setup(x => x.Ping(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("pong");            
            
        _factoryMock = new Mock<IRefitHttpClientFactory<IMediaServerApi>>();
        _factoryMock
            .Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(_restClientMock.Object);

        var mappingConfig = new MapperConfiguration(mc => { mc.AddProfile(new MapProfiles()); });
        var mapper = mappingConfig.CreateMapper();

        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        var logger = new Mock<ILogger<EmbyBaseHttpClient>>();
        _service = new EmbyBaseHttpClient(httpContextAccessorMock.Object, logger.Object, _factoryMock.Object, mapper);

        _service.SetDeviceInfo("embystat", "mediabrowser", "0", "c",
            "fa89fb6c-f3b7-4cc5-bc17-9522e3b94246");
        _service.BaseUrl = "localhost:9000";
        _service.ApiKey = "apikey";
        _authorizationParameter =
            "mediabrowser RestClient=\"other\", DeviceId=\"c\", Device=\"embystat\", Version=\"0\"";
    }

    [Fact]
    public async Task GetMediaFolders_Should_Return_List_Of_Media_Folders()
    {
        var result = await _service.GetLibraries();
        result.Should().NotBeNull();
        result.Length.Should().Be(1);

        result[0].Id.Should().Be("1");
        result[0].Name.Should().Be("Movies");

        _restClientMock.Verify(x => x.GetMediaFolders(_service.ApiKey, _authorizationParameter));
        _restClientMock.VerifyNoOtherCalls();

        _factoryMock.Verify(x => x.CreateClient(_service.BaseUrl));
        _factoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetInstalledPlugins_Should_Return_List_Of_Plugins()
    {
        var result = await _service.GetInstalledPlugins();
        result.Should().NotBeNull();
        result.Count.Should().Be(1);

        result[0].Id.Should().Be("1");
        result[0].Name.Should().Be("testPlugin");

        _restClientMock.Verify(x => x.GetPlugins(_service.ApiKey, _authorizationParameter));
        _restClientMock.VerifyNoOtherCalls();

        _factoryMock.Verify(x => x.CreateClient(_service.BaseUrl));
        _factoryMock.VerifyNoOtherCalls();
    }
        
    [Fact]
    public async Task GetGenres_Should_Return_Genres()
    {
        var result = await _service.GetGenres();
        var list = result.ToList();
        list.Should().NotBeNull();

        list.Count.Should().Be(1);
        list[0].Id.Should().Be("1");
        list[0].Name.Should().Be("Action");
            
        _restClientMock.Verify(x => x.GetGenres(_service.ApiKey, _authorizationParameter, It.IsAny<ItemQuery>()));
        _restClientMock.VerifyNoOtherCalls();

        _factoryMock.Verify(x => x.CreateClient(_service.BaseUrl));
        _factoryMock.VerifyNoOtherCalls();
    }
        
    [Fact]
    public async Task GetPeople_Should_Return_People()
    {
        var result = await _service.GetPeople(0, 10);
        var list = result.ToList();
        list.Should().NotBeNull();

        list.Count.Should().Be(2);
        list[0].Id.Should().Be("1");
        list[0].Name.Should().Be("Will");
        list[1].Id.Should().Be("2");
        list[1].Name.Should().Be("John");

        _restClientMock.Verify(x => x.GetPeople(_service.ApiKey,
            _authorizationParameter,
            It.Is<ItemQuery>(y =>
                y.Recursive &&
                y.StartIndex == 0 &&
                y.Limit == 10)));
        _restClientMock.VerifyNoOtherCalls();

        _factoryMock.Verify(x => x.CreateClient(_service.BaseUrl));
        _factoryMock.VerifyNoOtherCalls();
    }
        
    [Fact]
    public async Task GetPeopleCount_Should_Return_People()
    {
        var result = await _service.GetPeopleCount();

        result.Should().Be(2);

        _restClientMock.Verify(x => x.GetPeople(_service.ApiKey,
            _authorizationParameter,
            It.Is<ItemQuery>(y =>
                y.Recursive &&
                y.EnableTotalRecordCount &&
                y.Limit == 0)));
        _restClientMock.VerifyNoOtherCalls();

        _factoryMock.Verify(x => x.CreateClient(_service.BaseUrl));
        _factoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetServerInfo_Should_Return_Server_Info()
    {
        var result = await _service.GetServerInfo();
        result.Should().NotBeNull();

        result.ServerName.Should().Be("server-name");

        _restClientMock.Verify(x => x.GetServerInfo(_service.ApiKey, _authorizationParameter));
        _restClientMock.VerifyNoOtherCalls();

        _factoryMock.Verify(x => x.CreateClient(_service.BaseUrl));
        _factoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetServerInfo_Should_Return_Null_If_Failed()
    {
        var restClientMock = new Mock<IMediaServerApi>();
        restClientMock
            .Setup(x => x.GetServerInfo(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new ApiResponse<MediaServerInfo>(new HttpResponseMessage(HttpStatusCode.NotFound),
                new MediaServerInfo {ServerName = "server-name"}, new RefitSettings()));


        var factoryMock = new Mock<IRefitHttpClientFactory<IMediaServerApi>>();
        factoryMock
            .Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(restClientMock.Object);

        var mappingConfig = new MapperConfiguration(mc => { mc.AddProfile(new MapProfiles()); });
        var mapper = mappingConfig.CreateMapper();

        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        var logger = new Mock<ILogger<EmbyBaseHttpClient>>();

        var service = new EmbyBaseHttpClient(httpContextAccessorMock.Object, logger.Object, factoryMock.Object, mapper);

        service.SetDeviceInfo("embystat", "mediabrowser", "0", "c",
            "fa89fb6c-f3b7-4cc5-bc17-9522e3b94246");
        service.BaseUrl = "localhost:9000";
        service.ApiKey = "apikey";

        var result = await service.GetServerInfo();
        result.Should().BeNull();

        restClientMock.Verify(x => x.GetServerInfo(service.ApiKey, _authorizationParameter));
        restClientMock.VerifyNoOtherCalls();

        factoryMock.Verify(x => x.CreateClient(service.BaseUrl));
        factoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetUsers_Should_Return_List_Of_Users()
    {
        var result = await _service.GetUsers();
        result.Should().NotBeNull();
        result.Length.Should().Be(1);

        result[0].Id.Should().Be("1");
        result[0].Name.Should().Be("Mike");

        _restClientMock.Verify(x => x.GetUsers(_service.ApiKey, _authorizationParameter));
        _restClientMock.VerifyNoOtherCalls();

        _factoryMock.Verify(x => x.CreateClient(_service.BaseUrl));
        _factoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetDevices_Should_Return_List_Of_Devices()
    {
        var result = await _service.GetDevices();
        var list = result.ToList();
        list.Should().NotBeNull();
        list.Count.Should().Be(1);

        list[0].Id.Should().Be("1");
        list[0].Name.Should().Be("Web");

        _restClientMock.Verify(x => x.GetDevices(_service.ApiKey, _authorizationParameter));
        _restClientMock.VerifyNoOtherCalls();

        _factoryMock.Verify(x => x.CreateClient(_service.BaseUrl));
        _factoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetMedia_Should_Return_List_Of_Movies()
    {
        var result = await _service.GetMedia<Movie>("1", 0, 10, null, "Movie");

        var list = result.ToList();
        list.Should().NotBeNull();
        list.Count.Should().Be(1);

        list[0].Id.Should().Be("1");
        list[0].Name.Should().Be("Lord of the rings");

        _restClientMock.Verify(x => x.GetItems(_service.ApiKey,
            _authorizationParameter,
            It.Is<Dictionary<string, string>>(x =>
                x["ParentId"] == "1" &&
                x["StartIndex"] == "0" &&
                x["Limit"] == "10")));
        _restClientMock.VerifyNoOtherCalls();

        _factoryMock.Verify(x => x.CreateClient(_service.BaseUrl));
        _factoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetShows_Should_Return_List_Of_Shows()
    {
        var result = await _service.GetShows("1", 0, 10, null);

        var list = result.ToList();
        list.Should().NotBeNull();
        list.Count.Should().Be(1);

        list[0].Id.Should().Be("1");
        list[0].Name.Should().Be("Lord of the rings");

        _restClientMock.Verify(x => x.GetItems(_service.ApiKey,
            _authorizationParameter,
            It.Is<Dictionary<string, string>>(x =>
                x["ParentId"] == "1" &&
                x["StartIndex"] == "0" &&
                x["Limit"] == "10" &&
                x["IncludeItemTypes"] == "Series")));
        _restClientMock.VerifyNoOtherCalls();

        _factoryMock.Verify(x => x.CreateClient(_service.BaseUrl));
        _factoryMock.VerifyNoOtherCalls();
    }
    
    [Fact]
    public async Task GetShows_ByShowIds_Should_Return_List_Of_Shows()
    {
        var ids = new[] {"1", "2"};
        var result = await _service.GetShows(ids, 0, 10);

        var list = result.ToList();
        list.Should().NotBeNull();
        list.Count.Should().Be(1);

        list[0].Id.Should().Be("1");
        list[0].Name.Should().Be("Lord of the rings");

        _restClientMock.Verify(x => x.GetItems(_service.ApiKey,
            _authorizationParameter,
            It.Is<Dictionary<string, string>>(x =>
                x["Ids"] == "1,2" &&
                x["StartIndex"] == "0" &&
                x["Limit"] == "10" &&
                x["IncludeItemTypes"] == "Series")));
        _restClientMock.VerifyNoOtherCalls();

        _factoryMock.Verify(x => x.CreateClient(_service.BaseUrl));
        _factoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetSeasons_Should_Return_List_Of_Season()
    {
        var result = await _service.GetSeasons("1");

        var list = result.ToList();
        list.Should().NotBeNull();
        list.Count.Should().Be(1);

        list[0].Id.Should().Be("1");
        list[0].Name.Should().Be("Lord of the rings");

        _restClientMock.Verify(x => x.GetItems(_service.ApiKey,
            _authorizationParameter,
            It.Is<Dictionary<string, string>>(x =>
                x["ParentId"] == "1" &&
                x["IncludeItemTypes"] == "Season")));
        _restClientMock.VerifyNoOtherCalls();

        _factoryMock.Verify(x => x.CreateClient(_service.BaseUrl));
        _factoryMock.VerifyNoOtherCalls();
    }
        
    [Fact]
    public async Task GetEpisodes_Should_Return_List_Of_Episodes()
    {
        var result = await _service.GetEpisodes("1");

        var list = result.ToList();
        list.Should().NotBeNull();
        list.Count.Should().Be(1);

        list[0].Id.Should().Be("1");
        list[0].Name.Should().Be("Lord of the rings");

        _restClientMock.Verify(x => x.GetItems(_service.ApiKey,
            _authorizationParameter,
            It.Is<Dictionary<string, string>>(x =>
                x["ParentId"] == "1" &&
                x["IncludeItemTypes"] == "Episode")));
        _restClientMock.VerifyNoOtherCalls();

        _factoryMock.Verify(x => x.CreateClient(_service.BaseUrl));
        _factoryMock.VerifyNoOtherCalls();
    }
}