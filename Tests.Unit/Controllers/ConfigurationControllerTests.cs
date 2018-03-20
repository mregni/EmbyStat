using AutoMapper;
using EmbyStat.Controllers.Configuration;
using EmbyStat.Controllers.Helpers;
using EmbyStat.Repositories.Config;
using EmbyStat.Services.Config;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Tests.Unit.Controllers
{
	public class ConfigurationControllerTests
	{
		private readonly ConfigurationController _subject;
		private readonly Configuration _configuration;
		private readonly Mock<IConfigurationService> _configurationServiceMock;
		private readonly Mock<ILogger<ConfigurationController>> _LoggerMock;

		public ConfigurationControllerTests()
		{
			 _configuration = new Configuration()
			{
				AccessToken = "1234567890",
				EmbyServerAddress = "https://localhost:8096",
				EmbyUserName = "admin",
				Id = "0987654321",
				Language = "en",
				UserId = "12345",
				Username = "usernameAdmin",
				WizardFinished = false
			};

			_configurationServiceMock = new Mock<IConfigurationService>();
			_configurationServiceMock.Setup(x => x.GetServerSettings()).Returns(_configuration);

			_LoggerMock = new Mock<ILogger<ConfigurationController>>();

			_subject = new ConfigurationController(_configurationServiceMock.Object, _LoggerMock.Object);
			Mapper.Initialize(cfg => cfg.AddProfile<MapProfiles>());
		}

		[Fact]
		public void TestIfConfigurationIsLoadedCorrect()
		{
			var result = _subject.Get();

			result.Should().BeOfType<OkObjectResult>();
			_configurationServiceMock.Verify(x => x.GetServerSettings(), Times.Once);
		}
	}
}
