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
		    var configuration = new Dictionary<string, string>
		    {
		        { Constants.Configuration.EmbyUserId, "09876" },
		        { Constants.Configuration.Language, "en-US" },
		        { Constants.Configuration.UserName, "admin" },
		        { Constants.Configuration.WizardFinished, "true" },
		        { Constants.Configuration.EmbyServerAddress, "http://localhost" },
		        { Constants.Configuration.AccessToken, "1234567890" },
		        { Constants.Configuration.EmbyUserName, "reggi" },
		        { Constants.Configuration.ToShortMovie, "10" }
		    };

			_configurationRepositoryMock = new Mock<IConfigurationRepository>();
		    _configurationRepositoryMock.Setup(x => x.GetConfiguration()).Returns(configuration);

		    _subject = new ConfigurationService(_configurationRepositoryMock.Object);
		}

	    [Fact]
	    public void SaveSettings()
	    {
	        var config = new Dictionary<string, string>
	        {
	            { Constants.Configuration.EmbyUserId, "09876" },
	            { Constants.Configuration.Language, "en-US" },
	            { Constants.Configuration.UserName, "admin" },
	            { Constants.Configuration.WizardFinished, "true" },
	            { Constants.Configuration.EmbyServerAddress, "http://localhost" },
	            { Constants.Configuration.AccessToken, "1234567890" },
	            { Constants.Configuration.EmbyUserName, "reggi" },
	            { Constants.Configuration.ToShortMovie, "10" }
            };

            _subject.SaveServerSettings(config);

			_configurationRepositoryMock.Verify(x => x.GetConfiguration(), Times.Once);
			_configurationRepositoryMock.Verify(x => x.UpdateOrAdd(It.IsAny<Dictionary<string, string>>()), Times.Once);
	    }

	    [Fact]
	    public void GetServerSettingsFromRepository()
	    {
		    var settings = _subject.GetServerSettings();
		    settings.Should().NotBeNull();
		    settings[Constants.Configuration.EmbyUserId].Should().Be("09876");
	        settings[Constants.Configuration.Language].Should().Be("en-US");
	        settings[Constants.Configuration.UserName].Should().Be("admin");
	        settings[Constants.Configuration.WizardFinished].Should().Be("true");
	        settings[Constants.Configuration.EmbyServerAddress].Should().Be("http://localhost");
	        settings[Constants.Configuration.AccessToken].Should().Be("1234567890");
	        settings[Constants.Configuration.EmbyUserName].Should().Be("reggi");
	        settings[Constants.Configuration.ToShortMovie].Should().Be("10");
        }
    }
}
