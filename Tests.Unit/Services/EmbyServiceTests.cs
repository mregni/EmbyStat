using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmbyStat.Common.Exceptions;
using EmbyStat.Repositories.Config;
using EmbyStat.Repositories.EmbyPlugin;
using EmbyStat.Repositories.EmbyServerInfo;
using EmbyStat.Services.Emby;
using EmbyStat.Services.Emby.Models;
using EmbyStat.Services.EmbyClient;
using FluentAssertions;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Session;
using MediaBrowser.Model.System;
using MediaBrowser.Model.Users;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Tests.Unit.Services
{
	[Collection("Mapper collection")]
	public class EmbyServiceTests
    {
	    private readonly EmbyService _subject;
	    private readonly Mock<IEmbyClient> _embyClientMock;
	    private readonly Mock<IEmbyPluginRepository> _embyPluginRepositoryMock;
	    private readonly Mock<IEmbyServerInfoRepository> _embyServerInfoRepository;
		private readonly AuthenticationResult _authResult;
	    private readonly List<PluginInfo> _plugins;
	    private readonly ServerInfo _serverInfo;

	    public EmbyServiceTests()
	    {
		    _plugins = new List<PluginInfo>
		    {
			    new PluginInfo { Name = "EmbyStat plugin" },
			    new PluginInfo { Name = "Trakt plugin" }
		    };

			_authResult = new AuthenticationResult
			{
				AccessToken = "123456",
				ServerId = Guid.NewGuid().ToString(),
				SessionInfo = new SessionInfoDto(),
				User = new UserDto
				{
					ConnectUserName = "admin",
					Policy = new UserPolicy
					{
						IsAdministrator = true
					}
				}
			};

			_serverInfo = new ServerInfo
			{
				Id = Guid.NewGuid().ToString(),
				HttpServerPortNumber = 8096,
				HttpsPortNumber = 8097
			};

		    var systemInfo = new SystemInfo();
		    var loggerMock = new Mock<ILogger<EmbyService>>();

			_embyClientMock = new Mock<IEmbyClient>();
		    _embyClientMock.Setup(x => x.GetInstalledPluginsAsync()).Returns(Task.FromResult(_plugins));
		    _embyClientMock.Setup(x => x.SetAddressAndUrl(It.IsAny<string>(), It.IsAny<string>()));
		    _embyClientMock.Setup(x => x.GetServerInfo()).Returns(Task.FromResult(systemInfo));

		    _embyPluginRepositoryMock = new Mock<IEmbyPluginRepository>();
		    _embyPluginRepositoryMock.Setup(x => x.GetPlugins()).Returns(_plugins);
		    _embyPluginRepositoryMock.Setup(x => x.RemoveAllAndInsertPluginRange(It.IsAny<List<PluginInfo>>()));

		    var configurationRepositoryMock = new Mock<IConfigurationRepository>();
		    configurationRepositoryMock.Setup(x => x.GetSingle()).Returns(new Configuration());

		    _embyServerInfoRepository = new Mock<IEmbyServerInfoRepository>();
		    _embyServerInfoRepository.Setup(x => x.UpdateOrAdd(It.IsAny<ServerInfo>()));
		    _embyServerInfoRepository.Setup(x => x.GetSingle()).Returns(_serverInfo);

			_subject = new EmbyService(loggerMock.Object, _embyClientMock.Object, _embyPluginRepositoryMock.Object, configurationRepositoryMock.Object, _embyServerInfoRepository.Object);
	    }

	    [Fact]
	    public async void GetEmbyToken()
	    {
		    _embyClientMock.Setup(x => x.AuthenticateUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
			    .Returns(Task.FromResult(_authResult));

		    var login = new EmbyLogin
		    {
			    Address = "http://localhost",
			    UserName = "admin",
			    Password = "adminpass"
		    };

		    var token = await _subject.GetEmbyToken(login);

		    token.Username.Should().Be(_authResult.User.ConnectUserName);
		    token.Token.Should().Be(_authResult.AccessToken);
		    token.IsAdmin.Should().Be(_authResult.User.Policy.IsAdministrator);

		    _embyClientMock.Verify(x => x.AuthenticateUserAsync(
			    It.Is<string>(y => y == login.UserName ),
			    It.Is<string>(y => y == login.Password),
			    It.Is<string>(y => y == login.Address)));
	    }

	    [Fact]
		public async void GetEmbyTokenWithNoLoginInfo()
	    {
		    BusinessException ex = await Assert.ThrowsAsync<BusinessException>(() => _subject.GetEmbyToken(null));

		    ex.Message.Should().Be("WRONG_USERNAME_OR_PASSWORD");
		    ex.StatusCode.Should().Be(500);
	    }

	    [Fact]
	    public async void GetEmbyTokenWithNoPassword()
	    {
			var login = new EmbyLogin
			{
				UserName = "Admin",
				Address = "http://localhost"
			};
		    BusinessException ex = await Assert.ThrowsAsync<BusinessException>(() => _subject.GetEmbyToken(login));

		    ex.Message.Should().Be("WRONG_USERNAME_OR_PASSWORD");
		    ex.StatusCode.Should().Be(500);
	    }

	    [Fact]
	    public async void GetEmbyTokenWithNoUserName()
	    {
		    var login = new EmbyLogin
		    {
			    Password = "AdminPass",
			    Address = "http://localhost"
		    };
		    BusinessException ex = await Assert.ThrowsAsync<BusinessException>(() => _subject.GetEmbyToken(login));

		    ex.Message.Should().Be("WRONG_USERNAME_OR_PASSWORD");
		    ex.StatusCode.Should().Be(500);
	    }

	    [Fact]
	    public async void GetEmbyTokenFailedLogin()
	    {
		    _embyClientMock.Setup(x => x.AuthenticateUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
			    .ThrowsAsync(new Exception());
			var login = new EmbyLogin
		    {
			    Password = "AdminPass",
			    Address = "http://localhost",
				UserName = "Admin"
		    };
		    BusinessException ex = await Assert.ThrowsAsync<BusinessException>(() => _subject.GetEmbyToken(login));

		    ex.Message.Should().Be("WRONG_USERNAME_OR_PASSWORD");
		    ex.StatusCode.Should().Be(500);
	    }

	    [Fact]
	    public void UpdateServerInfo()
	    {
		    _subject.FireSmallSyncEmbyServerInfo();

		    _embyClientMock.Verify(x => x.GetInstalledPluginsAsync(), Times.Once);
			_embyClientMock.Verify(x => x.GetServerInfo(), Times.Once);

		    _embyPluginRepositoryMock.Verify(x => x.RemoveAllAndInsertPluginRange(It.Is<List<PluginInfo>>(
			    y => y.Count == 2 &&
			         y.First().Name == _plugins.First().Name)));
			_embyServerInfoRepository.Verify(x => x.UpdateOrAdd(It.IsAny<ServerInfo>()));
		}

	    [Fact]
	    public void GetServerInfoFromDatabase()
	    {
		    var serverInfo = _subject.GetServerInfo();

		    serverInfo.Should().NotBeNull();
		    serverInfo.Id.Should().Be(_serverInfo.Id);
		    serverInfo.HttpServerPortNumber.Should().Be(_serverInfo.HttpServerPortNumber);
		    serverInfo.HttpsPortNumber.Should().Be(_serverInfo.HttpsPortNumber);
	    }
	}
}
