using System;
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
			var configuration = new Configuration()
		    {
			    AccessToken = "1234567890",
			    EmbyServerAddress = "https://localhost:8096",
			    EmbyUserName = "admin",
			    Id = "0987654321",
			    Language = "en",
			    EmbyUserId = "12345",
			    Username = "usernameAdmin",
			    WizardFinished = false
		    };

		    _configurationServiceMock = new Mock<IConfigurationService>();
		    _configurationServiceMock.Setup(x => x.GetServerSettings()).Returns(configuration);
		    _configurationServiceMock.Setup(x => x.SaveServerSettings(It.IsAny<Configuration>()));

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
			    WizardFinished = false
		    };

		    _subject.Update(configuration);

		    _configurationServiceMock.Verify(x => x.SaveServerSettings(It.Is<Configuration>(
			    y => y.WizardFinished == configuration.WizardFinished &&
			         y.AccessToken == configuration.AccessToken &&
			         y.EmbyServerAddress == configuration.EmbyServerAddress &&
			         y.EmbyUserName == configuration.EmbyUserName &&
			         y.Language == configuration.Language &&
			         y.EmbyUserId == configuration.EmbyUserId &&
			         y.Username == configuration.Username)), Times.Once);
		    _configurationServiceMock.Verify(x => x.GetServerSettings(), Times.Once);
	    }
	}
}
