using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.DataGrid;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.MediaServer;
using EmbyStat.Controllers;
using EmbyStat.Controllers.HelperClasses;
using EmbyStat.Controllers.MediaServer;
using EmbyStat.Core.MediaServers.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Tests.Unit.Builders;
using Xunit;
using MediaServerUser = EmbyStat.Common.Models.Entities.Users.MediaServerUser;

namespace Tests.Unit.Controllers;

public class MediaServerControllerTests
{
    private readonly Mapper _mapper;

    public MediaServerControllerTests()
    {
        var profiles = new MapProfiles();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(profiles));
        _mapper = new Mapper(configuration);
    }

    [Fact]
    public async Task TestApiKey_Should_Return_True_If_Token_Is_Valid()
    {
        var mediaServerServiceMock = new Mock<IMediaServerService>();
        mediaServerServiceMock
            .Setup(x => x.TestNewApiKey(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ServerType>()))
            .ReturnsAsync(true);

        var controller = new MediaServerController(mediaServerServiceMock.Object, _mapper);
        var loginViewModel = new LoginViewModel
        {
            Address = "http://localhost",
            ApiKey = "12345",
            Type = ServerType.Emby
        };

        var result = await controller.TestApiKey(loginViewModel);
        var resultObject = result.Should().BeOfType<OkObjectResult>().Subject.Value;

        var succeeded = resultObject.Should().BeOfType<bool>().Subject;
        succeeded.Should().BeTrue();

        mediaServerServiceMock.Verify(x => x.TestNewApiKey(loginViewModel.Address, loginViewModel.ApiKey,  ServerType.Emby), Times.Once);
        mediaServerServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task SearchEmby_Should_Return_Emby_Instance()
    {
        var mediaServer = new MediaServerUdpBroadcast
        {
            Id = "azerty",
            Address = "localhost",
            Name = "mediaServer",
            Port = 80,
            Protocol = 0
        };

        var mediaServerServiceMock = new Mock<IMediaServerService>();
        mediaServerServiceMock
            .Setup(x => x.SearchMediaServer(It.IsAny<ServerType>()))
            .ReturnsAsync(new[] {mediaServer});
        var controller = new MediaServerController(mediaServerServiceMock.Object, _mapper);
        var result = await controller.SearchMediaServer(0);

        var resultObject = result.Should().BeOfType<OkObjectResult>().Subject.Value;
        var mediaServerUdpBroadcasts = resultObject.Should().BeOfType<List<UdpBroadcastViewModel>>().Subject;

        mediaServerUdpBroadcasts.Count.Should().Be(1);
        mediaServerUdpBroadcasts[0].Address.Should().Be(mediaServer.Address);
        mediaServerUdpBroadcasts[0].Port.Should().Be(mediaServer.Port);
        mediaServerUdpBroadcasts[0].Protocol.Should().Be(mediaServer.Protocol);
        mediaServerUdpBroadcasts[0].Id.Should().Be(mediaServer.Id);
        mediaServerUdpBroadcasts[0].Name.Should().Be(mediaServer.Name);

        mediaServerServiceMock.Verify(x => x.SearchMediaServer(ServerType.Emby), Times.Once);
        mediaServerServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetServerInfo_Should_Return_Emby_Server_Info()
    {
        var serverInfoObject = new MediaServerInfo
        {
            HttpServerPortNumber = 8096,
            HttpsPortNumber = 8097
        };

        var mediaServerServiceMock = new Mock<IMediaServerService>();
        mediaServerServiceMock.Setup(x => x.GetServerInfo(false)).ReturnsAsync(serverInfoObject);
        mediaServerServiceMock.Setup(x => x.GetAllUsers()).ReturnsAsync(new []
        
        {
            new MediaServerUserBuilder("1").Build(),
            new MediaServerUserBuilder("2").Build(),
            new MediaServerUserBuilder("3").AddLastActivityDate(DateTime.Today.AddYears(-1)).Build(),
        });
        mediaServerServiceMock.Setup(x => x.GetAllDevices()).ReturnsAsync(new List<Device>
        {
            new DeviceBuilder("1").Build(),
            new DeviceBuilder("2").Build(),
            new DeviceBuilder("3").AddLastActivityDate(DateTime.Now.AddYears(-1)).Build()
        });
        var controller = new MediaServerController(mediaServerServiceMock.Object, _mapper);

        var result = await controller.GetServerInfo();
        var resultObject = result.Should().BeOfType<OkObjectResult>().Subject.Value;
        var serverInfo = resultObject.Should().BeOfType<ServerInfoViewModel>().Subject;

        serverInfo.Should().NotBeNull();
        serverInfo.HttpServerPortNumber.Should().Be(serverInfoObject.HttpServerPortNumber);
        serverInfo.HttpsPortNumber.Should().Be(serverInfoObject.HttpsPortNumber);
        serverInfo.ActiveDeviceCount.Should().Be(2);
        serverInfo.IdleDeviceCount.Should().Be(1);
        serverInfo.ActiveUserCount.Should().Be(2);
        serverInfo.IdleUserCount.Should().Be(1);
            
        mediaServerServiceMock.Verify(x => x.GetAllDevices(), Times.Once);
        mediaServerServiceMock.Verify(x => x.GetAllUsers(), Times.Once);
        mediaServerServiceMock.Verify(x => x.GetServerInfo(false), Times.Once);
        mediaServerServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetMediaServerStatus_Should_Return_MediaServer_Status()
    {
        var status = new MediaServerStatus {MissedPings = 0, Id = Guid.NewGuid()};
        var mediaServerServiceMock = new Mock<IMediaServerService>();
        mediaServerServiceMock.Setup(x => x.GetMediaServerStatus()).ReturnsAsync(status);
        var controller = new MediaServerController(mediaServerServiceMock.Object, _mapper);

        var result = await controller.GetMediaServerStatus();
        var resultObject = result.Should().BeOfType<OkObjectResult>().Subject.Value;
        var statusViewModel = resultObject.Should().BeOfType<EmbyStatusViewModel>().Subject;

        statusViewModel.Should().NotBeNull();
        statusViewModel.MissedPings.Should().Be(status.MissedPings);

        mediaServerServiceMock.Verify(x => x.GetMediaServerStatus(), Times.Once);
        mediaServerServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetMediaServerLibraries_Should_Return_MediaServer_Libraries()
    {
        var library = new Library
        {
            Type = LibraryType.Movies,
            Id = "000",
            Name = "Movies",
            Primary = "image1"
        };

        var mediaServerServiceMock = new Mock<IMediaServerService>();
        mediaServerServiceMock.Setup(x => x.GetMediaServerLibraries()).ReturnsAsync(new[] {library});
        var controller = new MediaServerController(mediaServerServiceMock.Object, _mapper);

        var result = await controller.GetMediaServerLibraries();
        var resultObject = result.Should().BeOfType<OkObjectResult>().Subject.Value;
        var libraryViewModel = resultObject.Should().BeOfType<List<LibraryViewModel>>().Subject;

        libraryViewModel.Should().NotBeNull();
        libraryViewModel.Count.Should().Be(1);
        libraryViewModel[0].Type.Should().Be((int) library.Type);
        libraryViewModel[0].Id.Should().Be(library.Id);
        libraryViewModel[0].Name.Should().Be(library.Name);
        libraryViewModel[0].Primary.Should().Be(library.Primary);

        mediaServerServiceMock.Verify(x => x.GetMediaServerLibraries(), Times.Once);
        mediaServerServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetUserPage_Should_Return_Page()
    {
        var page = new Page<MediaServerUserRow>(new[]
        {
            new MediaServerUserRow {Id = "12"},
            new MediaServerUserRow {Id = "32"}
        })
        {
            TotalCount = 2
        };

        var mediaServerServiceMock = new Mock<IMediaServerService>();
        mediaServerServiceMock
            .Setup(x => x.GetUserPage(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(page);
        
        var controller = new MediaServerController(mediaServerServiceMock.Object, _mapper);
        var result = await controller.GetUserPage(1, 10, "name", "asc", true);
        var resultObject = result.Should().BeOfType<OkObjectResult>().Subject.Value;
        var pageViewModel = resultObject.Should().BeOfType<PageViewModel<MediaServerUserRowViewModel>>().Subject;

        pageViewModel.Should().NotBeNull();
        pageViewModel.Data.Count().Should().Be(2);
        pageViewModel.TotalCount.Should().Be(2);
        
        mediaServerServiceMock.Verify(x => x.GetUserPage(1,10,"name", "asc", true), Times.Once);
        mediaServerServiceMock.VerifyNoOtherCalls();
    }
    
    [Fact]
    public async Task GetAdministrators_Should_Return_Administrators()
    {
        var users = new []
        {
            new MediaServerUser {Id = "12"},
            new MediaServerUser {Id = "32"}
        };

        var mediaServerServiceMock = new Mock<IMediaServerService>();
        mediaServerServiceMock
            .Setup(x => x.GetAllAdministrators())
            .ReturnsAsync(users);
        
        var controller = new MediaServerController(mediaServerServiceMock.Object, _mapper);
        var result = await controller.GetAdministrators();
        var resultObject = result.Should().BeOfType<OkObjectResult>().Subject.Value;
        var resultList = resultObject.Should().BeOfType<List<UserOverviewViewModel>>().Subject;

        resultList.Should().NotBeNull();
        resultList.Count.Should().Be(2);
        
        mediaServerServiceMock.Verify(x => x.GetAllAdministrators(), Times.Once);
        mediaServerServiceMock.VerifyNoOtherCalls();
    }
}