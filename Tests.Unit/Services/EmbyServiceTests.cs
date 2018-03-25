using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmbyStat.Common.Exceptions;
using EmbyStat.Repositories.EmbyPlugin;
using EmbyStat.Services.Emby;
using EmbyStat.Services.Emby.Models;
using EmbyStat.Services.EmbyClient;
using FluentAssertions;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Session;
using MediaBrowser.Model.Users;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Tests.Unit.Services
{
    public class EmbyServiceTests
    {
	    private readonly EmbyService _subject;
	    private readonly Mock<IEmbyClient> _embyClientMock;
	    private readonly Mock<IEmbyPluginRepository> _embyPluginRepositoryMock;
		private readonly AuthenticationResult _authResult;
	    private List<PluginInfo> _plugins;

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

			_embyClientMock = new Mock<IEmbyClient>();
		    _embyClientMock.Setup(x => x.GetInstalledPluginsAsync()).Returns(Task.FromResult(_plugins));
		    var loggerMock = new Mock<ILogger<EmbyService>>();

		    _embyPluginRepositoryMock = new Mock<IEmbyPluginRepository>();
		    _embyPluginRepositoryMock.Setup(x => x.GetPlugins()).Returns(_plugins);
		    _embyPluginRepositoryMock.Setup(x => x.InsertPluginRange(It.IsAny<List<PluginInfo>>()));
			_subject = new EmbyService(loggerMock.Object, _embyClientMock.Object, _embyPluginRepositoryMock.Object);
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
		    _subject.UpdateServerInfo();

		    _embyClientMock.Verify(x => x.GetInstalledPluginsAsync(), Times.Once);
		    _embyPluginRepositoryMock.Verify(x => x.InsertPluginRange(It.Is<List<PluginInfo>>(
			    y => y.Count == 2 &&
			         y.First().Name == _plugins.First().Name)));

		}

	    [Fact]
	    public void GetPluginsFromDatabase()
	    {
		    var plugins = _subject.GetInstalledPlugins();

		    plugins.Should().NotBeNull();
		    plugins.Count.Should().Be(2);
		    plugins.First().Name.Should().Be(_plugins.First().Name);
		    plugins.Skip(1).First().Name.Should().Be(_plugins.Skip(1).First().Name);

			_embyPluginRepositoryMock.Verify(x => x.GetPlugins(), Times.Once);
		}
	}
}
