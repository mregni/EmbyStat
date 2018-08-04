using System.Collections.Generic;
using EmbyStat.Common;
using EmbyStat.Common.Models;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace Tests.Unit.Services
{
	[Collection("Services collection")]
	public class ConfigurationServiceTests
    {
	    private readonly ConfigurationService _subject;
	    private readonly Mock<IConfigurationRepository> _configurationRepositoryMock;

	    public ConfigurationServiceTests()
	    {
	        var configuration = new List<ConfigurationKeyValue>
	        {
	            new ConfigurationKeyValue{ Id = Constants.Configuration.EmbyUserId, Value = "09876" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.Language, Value = "en-US" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.UserName, Value = "admin" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.WizardFinished, Value = "True" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.EmbyServerAddress, Value = "http://localhost" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.AccessToken, Value = "1234567890" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.EmbyUserName, Value = "reggi" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.ToShortMovie, Value = "10" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.ServerName, Value = "ServerName" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.LastTvdbUpdate, Value = "09/10/2018" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.MovieCollectionTypes, Value = "[1, 2]" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.ShowCollectionTypes, Value = "[1, 2, 3]" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.TvdbApiKey, Value = "qsdfqsdfqsdfqsdqsdf" }
            };

			_configurationRepositoryMock = new Mock<IConfigurationRepository>();
		    _configurationRepositoryMock.Setup(x => x.GetConfiguration()).Returns(new Configuration(configuration));

		    _subject = new ConfigurationService(_configurationRepositoryMock.Object);
		}

	    [Fact]
	    public void SaveSettings()
	    {
	        var configuration = new List<ConfigurationKeyValue>
	        {
	            new ConfigurationKeyValue{ Id = Constants.Configuration.EmbyUserId, Value = "09876" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.Language, Value = "en-US" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.UserName, Value = "admin" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.WizardFinished, Value = "True" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.EmbyServerAddress, Value = "http://localhost" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.AccessToken, Value = "1234567890" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.EmbyUserName, Value = "reggi" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.ToShortMovie, Value = "10" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.ServerName, Value = "ServerName" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.LastTvdbUpdate, Value = "09/10/2018" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.MovieCollectionTypes, Value = "[1, 2]" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.ShowCollectionTypes, Value = "[1, 2, 3]" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.TvdbApiKey, Value = "qsdfqsdfqsdfqsdqsdf" }
            };

            _subject.SaveServerSettings(new Configuration(configuration));

			_configurationRepositoryMock.Verify(x => x.GetConfiguration(), Times.Once);
			_configurationRepositoryMock.Verify(x => x.Update(It.IsAny<Configuration>()), Times.Once);
	    }

	    [Fact]
	    public void GetServerSettingsFromRepository()
	    {
		    var settings = _subject.GetServerSettings();
		    settings.Should().NotBeNull();
		    settings.EmbyUserId.Should().Be("09876");
	        settings.Language.Should().Be("en-US");
	        settings.Username.Should().Be("admin");
	        settings.WizardFinished.Should().Be(true);
	        settings.EmbyServerAddress.Should().Be("http://localhost");
	        settings.AccessToken.Should().Be("1234567890");
	        settings.EmbyUserName.Should().Be("reggi");
	        settings.ToShortMovie.Should().Be(10);
	        settings.ServerName.Should().Be("ServerName");
	        settings.LastTvdbUpdate.Should().NotBeNull();
	        settings.LastTvdbUpdate.Value.Day.Should().Be(13);
	        settings.LastTvdbUpdate.Value.Month.Should().Be(10);
	        settings.LastTvdbUpdate.Value.Year.Should().Be(2018);
	        settings.MovieCollectionTypes.Count.Should().Be(2);
	        settings.ShowCollectionTypes.Count.Should().Be(3);
        }
    }
}
