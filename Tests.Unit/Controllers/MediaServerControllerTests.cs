using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Controllers;
using EmbyStat.Controllers.HelperClasses;
using EmbyStat.Controllers.MediaServer;
using EmbyStat.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Tests.Unit.Controllers
{
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
        public void TestApiKey_Should_Return_True_If_Token_Is_Valid()
        {
            var mediaServerServiceMock = new Mock<IMediaServerService>();
            mediaServerServiceMock.Setup(x => x.TestNewApiKey(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ServerType>())).Returns(true);

            var controller = new MediaServerController(mediaServerServiceMock.Object, _mapper);
            var loginViewModel = new LoginViewModel
            {
                Address = "http://localhost",
                ApiKey = "12345"
            };

            var result = controller.TestApiKey(loginViewModel);
            var resultObject = result.Should().BeOfType<OkObjectResult>().Subject.Value;

            var succeeded = resultObject.Should().BeOfType<bool>().Subject;
            succeeded.Should().BeTrue();

            controller.Dispose();
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
                .ReturnsAsync(new[] { mediaServer });
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
            controller.Dispose();
        }

        [Fact]
        public void GetServerInfo_Should_Return_Emby_Server_Info()
        {
            var serverInfoObject = new ServerInfo
            {
                HttpServerPortNumber = 8096,
                HttpsPortNumber = 8097
            };

            var mediaServerServiceMock = new Mock<IMediaServerService>();
            mediaServerServiceMock.Setup(x => x.GetServerInfo(false)).Returns(serverInfoObject);
            var controller = new MediaServerController(mediaServerServiceMock.Object, _mapper);

            var result = controller.GetServerInfo();
            var resultObject = result.Should().BeOfType<OkObjectResult>().Subject.Value;
            var serverInfo = resultObject.Should().BeOfType<ServerInfoViewModel>().Subject;

            serverInfo.Should().NotBeNull();
            serverInfo.HttpServerPortNumber.Should().Be(serverInfoObject.HttpServerPortNumber);
            serverInfo.HttpsPortNumber.Should().Be(serverInfoObject.HttpsPortNumber);
        }

        [Fact]
        public void GetMediaServerStatus_Should_Return_MediaServer_Status()
        {
            var status = new MediaServerStatus { MissedPings = 0, Id = Guid.NewGuid() };
            var mediaServerServiceMock = new Mock<IMediaServerService>();
            mediaServerServiceMock.Setup(x => x.GetMediaServerStatus()).Returns(status);
            var controller = new MediaServerController(mediaServerServiceMock.Object, _mapper);

            var result = controller.GetMediaServerStatus();
            var resultObject = result.Should().BeOfType<OkObjectResult>().Subject.Value;
            var statusViewModel = resultObject.Should().BeOfType<EmbyStatusViewModel>().Subject;

            statusViewModel.Should().NotBeNull();
            statusViewModel.MissedPings.Should().Be(status.MissedPings);

            mediaServerServiceMock.Verify(x => x.GetMediaServerStatus(), Times.Once);
        }

        [Fact]
        public void GetMediaServerLibraries_Should_Return_MediaServer_Libraries()
        {
            var library = new Library
            {
                Type = LibraryType.Movies,
                Id = "000",
                Name = "Movies",
                Primary = "image1"
            };

            var mediaServerServiceMock = new Mock<IMediaServerService>();
            mediaServerServiceMock.Setup(x => x.GetMediaServerLibraries()).Returns(new[] { library });
            var controller = new MediaServerController(mediaServerServiceMock.Object, _mapper);

            var result = controller.GetMediaServerLibraries();
            var resultObject = result.Should().BeOfType<OkObjectResult>().Subject.Value;
            var libraryViewModel = resultObject.Should().BeOfType<List<LibraryViewModel>>().Subject;

            libraryViewModel.Should().NotBeNull();
            libraryViewModel.Count.Should().Be(1);
            libraryViewModel[0].Type.Should().Be((int)library.Type);
            libraryViewModel[0].Id.Should().Be(library.Id);
            libraryViewModel[0].Name.Should().Be(library.Name);
            libraryViewModel[0].PrimaryImage.Should().Be(library.Primary);

            mediaServerServiceMock.Verify(x => x.GetMediaServerLibraries(), Times.Once);
        }
    }
}
