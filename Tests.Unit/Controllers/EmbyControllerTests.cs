using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Controllers.Emby;
using EmbyStat.Controllers.Helpers;
using EmbyStat.Services.Emby;
using EmbyStat.Services.Emby.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Tests.Unit.Controllers
{
	[Collection("Controller collection")]
	public class EmbyControllerTests : IDisposable
    {
	    private readonly EmbyController _subject;
	    private readonly Mock<IEmbyService> _embyServiceMock;

	    public EmbyControllerTests()
	    {
			var token = new EmbyToken()
		    {
			    IsAdmin = true,
			    Token = "azerty",
			    Username = "admin"
		    };

		    var emby = new EmbyUdpBroadcast()
		    {
			    Id = "azerty",
			    Address = "http://localhost",
			    Name = "emby"
		    };

			_embyServiceMock = new Mock<IEmbyService>();
		    _embyServiceMock.Setup(x => x.GetEmbyToken(It.IsAny<EmbyLogin>())).Returns(Task.FromResult(token));
		    _embyServiceMock.Setup(x => x.SearchEmby()).Returns(emby);

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

		    result.Should().BeOfType<OkObjectResult>();
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

		    result.Should().BeOfType<OkObjectResult>();
		    _embyServiceMock.Verify(x => x.SearchEmby(), Times.Once);
	    }
    }
}
