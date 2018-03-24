using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Repositories.Config;
using EmbyStat.Services.Config;
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
	    private Configuration _configuration;

		public ConfigurationServiceTests()
	    {
			_configuration = new Configuration
			{
				Id = "1234567",
				UserId = "09876",
				Language = "en",
				Username = "admin",
				WizardFinished = true,
				EmbyServerAddress = "http://localhost",
				AccessToken = "1234567890",
				EmbyUserName = "reggi"
			};

			_configurationRepositoryMock = new Mock<IConfigurationRepository>();
		    _configurationRepositoryMock.Setup(x => x.GetSingle()).Returns(_configuration);

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
			_configurationRepositoryMock.Verify(x => x.Update(It.Is<Configuration>(
				y => y.WizardFinished == config.WizardFinished &&
				     y.AccessToken == config.AccessToken &&
				     y.EmbyServerAddress == config.EmbyServerAddress &&
				     y.EmbyUserName == config.EmbyUserName &&
				     y.Language == config.Language && 
				     y.Username == config.Username)));
	    }
    }
}
