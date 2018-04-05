using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Controllers.Emby;
using EmbyStat.Repositories.EmbyDrive;
using EmbyStat.Repositories.EmbyServerInfo;
using EmbyStat.Services.Emby;
using EmbyStat.Services.Emby.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Tests.Unit.Controllers
{
	[Collection("Mapper collection")]
	public class EmbyControllerTests : IDisposable
    {
	    private readonly EmbyController _subject;
	    private readonly Mock<IEmbyService> _embyServiceMock;

	    private readonly EmbyToken _token;
	    private readonly ServerInfo _serverInfo;
	    private readonly EmbyUdpBroadcast _emby;
	    private readonly List<Drives> _drives;

		public EmbyControllerTests()
	    {
		    _token = new EmbyToken()
		    {
			    IsAdmin = true,
			    Token = "azerty",
			    Username = "admin"
		    };

		    _emby = new EmbyUdpBroadcast()
		    {
			    Id = "azerty",
			    Address = "http://localhost",
			    Name = "emby"
		    };

			_serverInfo = new ServerInfo
			{
				Id = Guid.NewGuid().ToString(),
				HttpServerPortNumber = 8096,
				HttpsPortNumber = 8097
			};

			_drives = new List<Drives>
			{
				new Drives(), new Drives()
			};

			_embyServiceMock = new Mock<IEmbyService>();
		    _embyServiceMock.Setup(x => x.GetEmbyToken(It.IsAny<EmbyLogin>())).Returns(Task.FromResult(_token));
		    _embyServiceMock.Setup(x => x.SearchEmby()).Returns(_emby);
		    _embyServiceMock.Setup(x => x.FireSmallSyncEmbyServerInfo());
		    _embyServiceMock.Setup(x => x.GetServerInfo()).Returns(_serverInfo);
		    _embyServiceMock.Setup(x => x.GetLocalDrives()).Returns(_drives);

			var loggerMock = new Mock<ILogger<EmbyController>>();

		    _subject = new EmbyController(_embyServiceMock.Object, loggerMock.Object);
		}

	    public void Dispose()
	    {
		    _subject?.Dispose();
		}

		[Fact]
	    public async void IsEmbyTokenReturned()
	    {
		    var loginViewModel = new EmbyLoginViewModel
		    {
			    Address = "http://localhost",
			    Password = "password",
			    UserName = "username"
		    };

		    var result = await _subject.GenerateToken(loginViewModel);

		    var tokenObject = result.Should().BeOfType<OkObjectResult>().Subject.Value;
		    var token = tokenObject.Should().BeOfType<EmbyTokenViewModel>().Subject;

		    token.IsAdmin.Should().Be(_token.IsAdmin);
		    token.Token.Should().Be(_token.Token);
		    token.Username.Should().Be(_token.Username);

		    _embyServiceMock.Verify(x => x.GetEmbyToken(It.Is<EmbyLogin>(
			    y => y.UserName == loginViewModel.UserName &&
			         y.Address == loginViewModel.Address &&
			         y.Password == loginViewModel.Password)
		    ), Times.Once);
	    }

	    [Fact]
	    public void IsEmbyReturned()
	    {
		    var result = _subject.SearchEmby();

		    var resultObject = result.Should().BeOfType<OkObjectResult>().Subject.Value;
		    var embyUdpBroadcast = resultObject.Should().BeOfType<EmbyUdpBroadcastViewModel>().Subject;

		    embyUdpBroadcast.Address.Should().Be(_emby.Address);
		    embyUdpBroadcast.Id.Should().Be(_emby.Id);
		    embyUdpBroadcast.Name.Should().Be(_emby.Name);

			_embyServiceMock.Verify(x => x.SearchEmby(), Times.Once);
	    }

	    [Fact]
	    public void IsServerInfoUpdated()
	    {
		    var result = _subject.FireSmallEmbySync();

		    result.Should().BeOfType<OkResult>();
		    _embyServiceMock.Verify(x => x.FireSmallSyncEmbyServerInfo(), Times.Once);
	    }

	    [Fact]
	    public void IsServerInfoReturned()
	    {
		    var result = _subject.GetServerInfo();
		    var resultObject = result.Should().BeOfType<OkObjectResult>().Subject.Value;
		    var serverInfo = resultObject.Should().BeOfType<ServerInfoViewModel>().Subject;

		    serverInfo.Should().NotBeNull();
		    serverInfo.Drives.Count.Should().Be(2);
		    serverInfo.HttpServerPortNumber.Should().Be(_serverInfo.HttpServerPortNumber);
		    serverInfo.HttpsPortNumber.Should().Be(_serverInfo.HttpsPortNumber);
	    }
	}
}
