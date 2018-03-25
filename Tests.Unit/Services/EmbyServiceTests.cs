using System;
using System.Threading.Tasks;
using EmbyStat.Common.Exceptions;
using EmbyStat.Services.Emby;
using EmbyStat.Services.Emby.Models;
using EmbyStat.Services.EmbyClient;
using FluentAssertions;
using MediaBrowser.Model.Dto;
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
	    private readonly AuthenticationResult _authResult;

	    public EmbyServiceTests()
	    {

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
		    var loggerMock = new Mock<ILogger<EmbyService>>();
		    _subject = new EmbyService(loggerMock.Object, _embyClientMock.Object);
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
		public async Task GetEmbyTokenWithNoLoginInfo()
	    {
		    BusinessException ex = await Assert.ThrowsAsync<BusinessException>(() => _subject.GetEmbyToken(null));

		    ex.Message.Should().Be("WRONG_USERNAME_OR_PASSWORD");
		    ex.StatusCode.Should().Be(500);
	    }

	    [Fact]
	    public async Task GetEmbyTokenWithNoPassword()
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
	    public async Task GetEmbyTokenWithNoUserName()
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
	    public async Task GetEmbyTokenFailedLogin()
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
	}
}
