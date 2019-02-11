using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Clients.EmbyClient.Model;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Controllers;
using EmbyStat.Controllers.ViewModels.Emby;
using EmbyStat.Controllers.ViewModels.Server;
using EmbyStat.Services.Interfaces;
using EmbyStat.Services.Models.Emby;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
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
	    private readonly List<Drive> _drives;

		public EmbyControllerTests()
	    {
		    _token = new EmbyToken
            {
			    IsAdmin = true,
			    Token = "azerty",
			    Username = "admin"
		    };

		    _emby = new EmbyUdpBroadcast
            {
			    Id = "azerty",
			    Address = "localhost",
			    Name = "emby",
                Port = 80,
                Protocol = 0
		    };

			_serverInfo = new ServerInfo
			{
				HttpServerPortNumber = 8096,
				HttpsPortNumber = 8097
			};

			_drives = new List<Drive>
			{
				new Drive(), new Drive()
			};

			_embyServiceMock = new Mock<IEmbyService>();
		    _embyServiceMock.Setup(x => x.GetEmbyToken(It.IsAny<EmbyLogin>())).Returns(Task.FromResult(_token));
		    _embyServiceMock.Setup(x => x.SearchEmby()).Returns(_emby);
		    _embyServiceMock.Setup(x => x.GetServerInfo()).Returns(Task.FromResult(_serverInfo));
		    _embyServiceMock.Setup(x => x.GetLocalDrives()).Returns(_drives);

	        var _mapperMock = new Mock<IMapper>();
	        _mapperMock.Setup(x => x.Map<ServerInfoViewModel>(It.IsAny<ServerInfo>())).Returns(new ServerInfoViewModel { HttpServerPortNumber = 8096, HttpsPortNumber = 8097 });
            _mapperMock.Setup(x => x.Map<IList<DriveViewModel>>(It.IsAny<List<Drive>>())).Returns(new List<DriveViewModel>{ new DriveViewModel(), new DriveViewModel()});
            _mapperMock.Setup(x => x.Map<EmbyTokenViewModel>(It.IsAny<EmbyToken>())).Returns(new EmbyTokenViewModel{ IsAdmin = true, Token = "azerty", Username = "admin" });
	        _mapperMock.Setup(x => x.Map<EmbyUdpBroadcastViewModel>(It.IsAny<EmbyUdpBroadcast>())).Returns(new EmbyUdpBroadcastViewModel { Id = "azerty", Address = "localhost", Name = "emby", Protocol = 0, Port = 80});

		    _subject = new EmbyController(_embyServiceMock.Object, _mapperMock.Object);
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
	    }

	    [Fact]
	    public void IsEmbyReturned()
	    {
		    var result = _subject.SearchEmby();

		    var resultObject = result.Should().BeOfType<OkObjectResult>().Subject.Value;
		    var embyUdpBroadcast = resultObject.Should().BeOfType<EmbyUdpBroadcastViewModel>().Subject;

		    embyUdpBroadcast.Address.Should().Be(_emby.Address);
		    embyUdpBroadcast.Port.Should().Be(_emby.Port);
		    embyUdpBroadcast.Protocol.Should().Be(_emby.Protocol);
            embyUdpBroadcast.Id.Should().Be(_emby.Id);
		    embyUdpBroadcast.Name.Should().Be(_emby.Name);

			_embyServiceMock.Verify(x => x.SearchEmby(), Times.Once);
	    }

	    [Fact]
	    public void IsServerInfoReturned()
	    {
		    var result = _subject.GetServerInfo();
		    var resultObject = result.Result.Should().BeOfType<OkObjectResult>().Subject.Value;
		    var serverInfo = resultObject.Should().BeOfType<ServerInfoViewModel>().Subject;

		    serverInfo.Should().NotBeNull();
		    serverInfo.Drives.Count.Should().Be(2);
		    serverInfo.HttpServerPortNumber.Should().Be(_serverInfo.HttpServerPortNumber);
		    serverInfo.HttpsPortNumber.Should().Be(_serverInfo.HttpsPortNumber);
	    }
	}
}
