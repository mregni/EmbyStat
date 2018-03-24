using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;
using EmbyStat.Common.Exceptions;
using EmbyStat.Services.Emby;
using EmbyStat.Services.Emby.Models;
using EmbyStat.Services.EmbyClientFacade;
using FluentAssertions;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Session;
using MediaBrowser.Model.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Tests.Unit.Services
{
    public class EmbyServiceTests
    {
	    private readonly EmbyService _subject;
	    private readonly Mock<IEmbyClientFacade> _embyClientFacadeMock;
	    private AuthenticationResult _authResult;

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

			_embyClientFacadeMock = new Mock<IEmbyClientFacade>();
		    var loggerMock = new Mock<ILogger<EmbyService>>();
		    _subject = new EmbyService(loggerMock.Object, _embyClientFacadeMock.Object);
	    }

	    [Fact]
	    public async void GetEmbyToken()
	    {
		    _embyClientFacadeMock.Setup(x => x.AuthenticateUserAsync(It.IsAny<EmbyLogin>()))
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

		    _embyClientFacadeMock.Verify(x => x.AuthenticateUserAsync(It.Is<EmbyLogin>(
				y => y.Address == login.Address &&
				     y.Password == login.Password && 
				     y.UserName == login.UserName)));

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
		    _embyClientFacadeMock.Setup(x => x.AuthenticateUserAsync(It.IsAny<EmbyLogin>()))
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
