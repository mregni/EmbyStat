using System;
using System.Collections.Generic;
using EmbyStat.Common;
using EmbyStat.Common.Models;
using EmbyStat.Controllers;
using EmbyStat.Controllers.ViewModels.Configuration;
using EmbyStat.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Tests.Unit.Controllers
{
	[Collection("Mapper collection")]
	public class ConfigurationControllerTests : IDisposable
    {
	    private readonly ConfigurationController _subject;
	    private readonly Mock<IConfigurationService> _configurationServiceMock;

	    public ConfigurationControllerTests()
	    {
			var configuration = new Dictionary<string, string>
			{
			    { Constants.Configuration.EmbyUserId, "EmbyUserId" },
			    { Constants.Configuration.Language, "en-US" },
			    { Constants.Configuration.UserName, "admin" },
			    { Constants.Configuration.WizardFinished, "true" },
			    { Constants.Configuration.EmbyServerAddress, "http://localhost" },
			    { Constants.Configuration.AccessToken, "1234567980" },
			    { Constants.Configuration.EmbyUserName, "reggi" },
			    { Constants.Configuration.ToShortMovie, "10" },
			    { Constants.Configuration.ServerName, "ServerName" },
            };

            _configurationServiceMock = new Mock<IConfigurationService>();
		    _configurationServiceMock.Setup(x => x.GetServerSettings()).Returns(configuration);
		    _configurationServiceMock.Setup(x => x.SaveServerSettings(It.IsAny<Dictionary<string, string>>()));

			var loggerMock = new Mock<ILogger<ConfigurationController>>();

		    _subject = new ConfigurationController(_configurationServiceMock.Object, loggerMock.Object);
		}

	    public void Dispose()
	    {
		    _subject?.Dispose();
		}

		[Fact]
	    public void IsConfigurationLoaded()
	    {
		    var result = _subject.Get();

		    result.Should().BeOfType<OkObjectResult>();
		    _configurationServiceMock.Verify(x => x.GetServerSettings(), Times.Once);
	    }

	    [Fact]
	    public void IsConfigurationUpdatedCorrectly()
	    {
		    var configuration = new ConfigurationViewModel
		    {
			    AccessToken = "1234567890",
			    EmbyServerAddress = "https://localhost:8096",
			    EmbyUserName = "admin",
			    Language = "en",
			    EmbyUserId = "12345",
			    Username = "usernameAdmin",
		        ServerName = "ServerName",
                WizardFinished = false,
                ToShortMovie = 10
		    };

		    _subject.Update(configuration);

		    _configurationServiceMock.Verify(x => x.SaveServerSettings(It.IsAny<Dictionary<string, string>>()), Times.Once);
		    _configurationServiceMock.Verify(x => x.GetServerSettings(), Times.Once);
	    }
	}
}
