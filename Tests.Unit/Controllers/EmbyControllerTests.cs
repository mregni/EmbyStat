using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Controllers;
using EmbyStat.Controllers.Emby;
using EmbyStat.Services.Interfaces;
using EmbyStat.Services.Models.Emby;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Tests.Unit.Controllers
{
	public class EmbyControllerTests
    {
        private readonly Mapper _mapper;

        public EmbyControllerTests()
        {
            var profiles = new MapProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(profiles));
            _mapper = new Mapper(configuration);
        }

        [Fact]
	    public async Task TestApiKey_Should_Return_True_If_Token_Is_Valid()
        {
            var embyServiceMock = new Mock<IEmbyService>();
            embyServiceMock.Setup(x => x.TestNewEmbyApiKeyAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

            var controller = new EmbyController(embyServiceMock.Object, _mapper);
            var loginViewModel = new EmbyLoginViewModel
		    {
			    Address = "http://localhost",
			    ApiKey = "12345"
		    };

            var result = await controller.TestApiKey(loginViewModel);
            var resultObject = result.Should().BeOfType<OkObjectResult>().Subject.Value;
		    
            var succeeded = resultObject.Should().BeOfType<bool>().Subject;
            succeeded.Should().BeTrue();

            controller.Dispose();
        }

	    [Fact]
	    public void SearchEmby_Should_Return_Emby_Instance()
	    {
            var emby = new EmbyUdpBroadcast
            {
                Id = "azerty",
                Address = "localhost",
                Name = "emby",
                Port = 80,
                Protocol = 0
            };

            var embyServiceMock = new Mock<IEmbyService>();
            embyServiceMock.Setup(x => x.SearchEmby()).Returns(emby);
            var controller = new EmbyController(embyServiceMock.Object, _mapper);
            var result = controller.SearchEmby();

		    var resultObject = result.Should().BeOfType<OkObjectResult>().Subject.Value;
		    var embyUdpBroadcast = resultObject.Should().BeOfType<EmbyUdpBroadcastViewModel>().Subject;

            embyUdpBroadcast.Address.Should().Be(emby.Address);
		    embyUdpBroadcast.Port.Should().Be(emby.Port);
		    embyUdpBroadcast.Protocol.Should().Be(emby.Protocol);
            embyUdpBroadcast.Id.Should().Be(emby.Id);
		    embyUdpBroadcast.Name.Should().Be(emby.Name);

            embyServiceMock.Verify(x => x.SearchEmby(), Times.Once);
            controller.Dispose();
        }

        [Fact]
	    public async Task GetServerInfo_Should_Return_Emby_Server_Info()
	    {
            var serverInfoObject = new ServerInfo
            {
                HttpServerPortNumber = 8096,
                HttpsPortNumber = 8097
            };

            var embyServiceMock = new Mock<IEmbyService>();
            embyServiceMock.Setup(x => x.GetServerInfoAsync()).ReturnsAsync(serverInfoObject);
            var controller = new EmbyController(embyServiceMock.Object, _mapper);

            var result = await controller.GetServerInfo();
		    var resultObject = result.Should().BeOfType<OkObjectResult>().Subject.Value;
		    var serverInfo = resultObject.Should().BeOfType<ServerInfoViewModel>().Subject;

		    serverInfo.Should().NotBeNull();
		    serverInfo.HttpServerPortNumber.Should().Be(serverInfoObject.HttpServerPortNumber);
		    serverInfo.HttpsPortNumber.Should().Be(serverInfoObject.HttpsPortNumber);
	    }
	}
}
