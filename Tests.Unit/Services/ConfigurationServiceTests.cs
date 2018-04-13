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
		    var configuration = new Configuration
		    {
			    Id = "1234567",
			    EmbyUserId = "09876",
			    Language = "en",
			    Username = "admin",
			    WizardFinished = true,
			    EmbyServerAddress = "http://localhost",
			    AccessToken = "1234567890",
			    EmbyUserName = "reggi"
		    };

			_configurationRepositoryMock = new Mock<IConfigurationRepository>();
		    _configurationRepositoryMock.Setup(x => x.GetSingle()).Returns(configuration);

		    _subject = new ConfigurationService(_configurationRepositoryMock.Object);
		}

	    [Fact]
	    public void SaveSettings()
	    {
			var config = new Configuration
			{
				Language = "nl",
				Username = "adminUpdate",
				WizardFinished = false,
				EmbyServerAddress = "http://localhostUpdate",
				AccessToken = "1234567890Update",
				EmbyUserName = "reggiUpdate"
			};
			
			_subject.SaveServerSettings(config);

			_configurationRepositoryMock.Verify(x => x.GetSingle(), Times.Once);
			_configurationRepositoryMock.Verify(x => x.UpdateOrAdd(It.Is<Configuration>(
				y => y.WizardFinished == config.WizardFinished &&
				     y.AccessToken == config.AccessToken &&
				     y.EmbyServerAddress == config.EmbyServerAddress &&
				     y.EmbyUserName == config.EmbyUserName &&
				     y.Language == config.Language && 
				     y.Username == config.Username)));
	    }

	    [Fact]
	    public void GetServerSettingsFromRepository()
	    {
		    var settings = _subject.GetServerSettings();
		    settings.Should().NotBeNull();
		    settings.Id.Should().Be("1234567");
		    settings.EmbyUserId.Should().Be("09876");
		    settings.Language.Should().Be("en");
		    settings.Username.Should().Be("admin");
		    settings.WizardFinished.Should().BeTrue();
		    settings.EmbyServerAddress.Should().Be("http://localhost");
		    settings.AccessToken.Should().Be("1234567890");
		    settings.EmbyUserName.Should().Be("reggi");
		}
    }
}
