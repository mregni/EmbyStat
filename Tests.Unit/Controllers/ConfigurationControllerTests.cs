using System;
using System.Collections.Generic;
using AutoMapper;
using EmbyStat.Common;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Controllers;
using EmbyStat.Controllers.ViewModels.Configuration;
using EmbyStat.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
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
			var configuration = new List<ConfigurationKeyValue>
			{
			    new ConfigurationKeyValue{ Id = Constants.Configuration.EmbyUserId, Value = "EmbyUserId" },
			    new ConfigurationKeyValue{ Id = Constants.Configuration.Language, Value = "en-US" },
                new ConfigurationKeyValue{ Id = Constants.Configuration.UserName, Value = "admin" },
                new ConfigurationKeyValue{ Id = Constants.Configuration.WizardFinished, Value = "true" },
                new ConfigurationKeyValue{ Id = Constants.Configuration.EmbyServerAddress, Value = "http://localhost" },
                new ConfigurationKeyValue{ Id = Constants.Configuration.AccessToken, Value = "1234567980" },
                new ConfigurationKeyValue{ Id = Constants.Configuration.EmbyUserName, Value = "reggi" },
                new ConfigurationKeyValue{ Id = Constants.Configuration.ToShortMovie, Value = "10" },
                new ConfigurationKeyValue{ Id = Constants.Configuration.ServerName, Value = "ServerName" },
			    new ConfigurationKeyValue{ Id = Constants.Configuration.MovieCollectionTypes, Value = "[1, 2]" },
			    new ConfigurationKeyValue{ Id = Constants.Configuration.ShowCollectionTypes, Value = "[1, 2, 3]" }
            };

            _configurationServiceMock = new Mock<IConfigurationService>();
		    _configurationServiceMock.Setup(x => x.GetServerSettings()).Returns(new Configuration(configuration));
		    _configurationServiceMock.Setup(x => x.SaveServerSettings(It.IsAny<Configuration>()));

	        var _mapperMock = new Mock<IMapper>();

		    _subject = new ConfigurationController(_configurationServiceMock.Object, _mapperMock.Object);
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

		    _configurationServiceMock.Verify(x => x.SaveServerSettings(It.IsAny<Configuration>()), Times.Once);
		    _configurationServiceMock.Verify(x => x.GetServerSettings(), Times.Once);
	    }
	}
}
