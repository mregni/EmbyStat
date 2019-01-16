using System.Collections.Generic;
using EmbyStat.Common;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
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
	            new ConfigurationKeyValue{ Id = Constants.Configuration.EmbyServerAddress, Value = "localhost" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.AccessToken, Value = "1234567890" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.EmbyUserName, Value = "reggi" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.ToShortMovie, Value = "10" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.ServerName, Value = "ServerName" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.LastTvdbUpdate, Value = "09/10/2018" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.MovieCollectionTypes, Value = "[1, 2]" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.ShowCollectionTypes, Value = "[1, 2, 3]" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.TvdbApiKey, Value = "qsdfqsdfqsdfqsdqsdf" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.EmbyServerPort, Value = "8096" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.EmbyServerProtocol, Value = "0" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.AutoUpdate, Value = "True" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.UpdateTrain, Value = "2" }
            };

			_configurationRepositoryMock = new Mock<IConfigurationRepository>();
		    _configurationRepositoryMock.Setup(x => x.GetConfiguration()).Returns(new Configuration(configuration));

	        var _statisticsREpositoryMock = new Mock<IStatisticsRepository>();
	        _statisticsREpositoryMock.Setup(x => x.MarkShowTypesAsInvalid());
	        _statisticsREpositoryMock.Setup(x => x.MarkMovieTypesAsInvalid());

            var _appSettingsMock = new Mock<IOptions<AppSettings>>();
            _appSettingsMock.Setup(x => x.Value).Returns(new AppSettings(){Version = "0.0.0.0"});

            _subject = new ConfigurationService(_configurationRepositoryMock.Object, _statisticsREpositoryMock.Object, _appSettingsMock.Object);
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
	            new ConfigurationKeyValue{ Id = Constants.Configuration.EmbyServerAddress, Value = "localhost" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.AccessToken, Value = "1234567890" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.EmbyUserName, Value = "reggi" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.ToShortMovie, Value = "10" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.ServerName, Value = "ServerName" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.LastTvdbUpdate, Value = "09/10/2018" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.MovieCollectionTypes, Value = "[1, 2]" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.ShowCollectionTypes, Value = "[1, 2, 3]" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.TvdbApiKey, Value = "qsdfqsdfqsdfqsdqsdf" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.EmbyServerPort, Value = "8096" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.EmbyServerProtocol, Value = "0" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.AutoUpdate, Value = "True" },
	            new ConfigurationKeyValue{ Id = Constants.Configuration.UpdateTrain, Value = "2" }
            };

            _subject.SaveServerSettings(new Configuration(configuration));
            
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
	        settings.EmbyServerAddress.Should().Be("localhost");
	        settings.EmbyServerPort.Should().Be(8096);
	        settings.EmbyServerProtocol.Should().Be(0);
            settings.AccessToken.Should().Be("1234567890");
	        settings.EmbyUserName.Should().Be("reggi");
	        settings.ToShortMovie.Should().Be(10);
	        settings.ServerName.Should().Be("ServerName");
	        settings.LastTvdbUpdate.Should().NotBeNull();
	        settings.LastTvdbUpdate.Value.Day.Should().Be(9);
	        settings.LastTvdbUpdate.Value.Month.Should().Be(10);
	        settings.LastTvdbUpdate.Value.Year.Should().Be(2018);
	        settings.MovieCollectionTypes.Count.Should().Be(2);
	        settings.ShowCollectionTypes.Count.Should().Be(3);
	        settings.UpdateTrain.Should().Be(UpdateTrain.Release);
	        settings.AutoUpdate.Should().BeTrue();
            settings.Version.Should().Be("0.0.0.0");
        }
    }
}
