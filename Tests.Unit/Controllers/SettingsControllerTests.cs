
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Controllers.Settings;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Tests.Unit.Controllers
{
	[Collection("Mapper collection")]
	public class SettingsControllerTests : IDisposable
    {
	    private readonly SettingsController _subject;
	    private readonly Mock<ISettingsService> _settingsServiceMock;

        private Guid DeviceId { get; set; }

	    public SettingsControllerTests()
        {
            DeviceId = Guid.NewGuid();
            var settings = new UserSettings
			{
                Id = DeviceId,
                AppName = "EmbyStat",
                AutoUpdate = false,
                KeepLogsCount = 10,
                Language = "en-US",
                MovieLibraryTypes = new List<LibraryType>() { LibraryType.Other, LibraryType.Movies, LibraryType.HomeVideos },
                ShowLibraryTypes = new List<LibraryType>() { LibraryType.TvShow, LibraryType.Other },
                ToShortMovie = 10,
                UpdateInProgress = false,
                UpdateTrain = UpdateTrain.Beta,
                Username = "reggi",
                WizardFinished = true,
                Emby = new EmbySettings
                {
                    AccessToken = "1234567980",
                    UserId = "aaaaaaaaaa",
                    ServerName = "ServerName",
                    AuthorizationScheme = "MediaBrowser",
                    ServerAddress = "localhost",
                    ServerPort = 8097,
                    ServerProtocol = ConnectionProtocol.Https,
                    UserName = "admin"
                },
                Tvdb = new TvdbSettings
                {
                    LastUpdate = new DateTime(2019, 1, 1),
                    ApiKey = "ABCDE"
                }
            };

            _settingsServiceMock = new Mock<ISettingsService>();
		    _settingsServiceMock.Setup(x => x.GetAppSettings()).Returns(new AppSettings { Version = "0.0.0.0" });
		    _settingsServiceMock.Setup(x => x.GetUserSettings()).Returns(settings);
            _settingsServiceMock.Setup(x => x.SaveUserSettingsAsync(It.IsAny<UserSettings>()))
                .Returns(Task.FromResult(settings));

	        var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(x => x.Map<FullSettingsViewModel>(It.IsAny<UserSettings>()))
                .Returns(new FullSettingsViewModel());
            mapperMock.Setup(x => x.Map<UserSettings>(It.IsAny<FullSettingsViewModel>()))
                .Returns(new UserSettings()
                {
                    MovieLibraryTypes = new List<LibraryType>(),
                    ShowLibraryTypes = new List<LibraryType>()
                });

            var statisticsRepositoryMock = new Mock<IStatisticsRepository>();
            statisticsRepositoryMock.Setup(x => x.MarkMovieTypesAsInvalid());
            statisticsRepositoryMock.Setup(x => x.MarkShowTypesAsInvalid());

            var languageServiceMock = new Mock<ILanguageService>();

            _subject = new SettingsController(_settingsServiceMock.Object, statisticsRepositoryMock.Object, languageServiceMock.Object, mapperMock.Object);
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
		    _settingsServiceMock.Verify(x => x.GetUserSettings(), Times.Once);
	    }

	    [Fact]
	    public async void IsConfigurationUpdatedCorrectly()
	    {
		    var settings = new FullSettingsViewModel
		    {
                Id = DeviceId,
                AppName = "EmbyStat",
                AutoUpdate = false,
                KeepLogsCount = 10,
                Language = "en-US",
                MovieLibraryTypes = new List<int> { 0, 2, 6 },
                ShowLibraryTypes = new List<int> { 0, 2 },
                ToShortMovie = 10,
                UpdateInProgress = false,
                UpdateTrain = 0,
                Username = "reggi",
                WizardFinished = true,
                Emby = new FullSettingsViewModel.EmbySettingsViewModel
                {
                    AccessToken = "1234567980",
                    UserId = "aaaaaaaaaa",
                    ServerName = "ServerName",
                    AuthorizationScheme = "MediaBrowser",
                    ServerAddress = "localhost",
                    ServerPort = 8097,
                    ServerProtocol = 1,
                    UserName = "admin"
                },
                Tvdb = new FullSettingsViewModel.TvdbSettingsViewModel
                {
                    LastUpdate = new DateTime(2019, 1, 1),
                    ApiKey = "ABCDE"
                }
            };

		    await _subject.Update(settings);

		    _settingsServiceMock.Verify(x => x.SaveUserSettingsAsync(It.IsAny<UserSettings>()), Times.Once);
	    }
	}
}
