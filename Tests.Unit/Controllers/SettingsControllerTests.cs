﻿
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
using Tests.Unit.Builders.ViewModels;
using Xunit;

namespace Tests.Unit.Controllers
{
    public class SettingsControllerTests : IDisposable
    {
        private readonly SettingsController _subject;
        private readonly AppSettings _appSettings;
        private readonly UserSettings _userSettings;
        private readonly Mock<ISettingsService> _settingsServiceMock;

        private Guid DeviceId { get; set; }

        public SettingsControllerTests()
        {
            DeviceId = Guid.NewGuid();
            _userSettings = new UserSettings
            {
                Id = DeviceId,
                AppName = "EmbyStat",
                AutoUpdate = false,
                KeepLogsCount = 10,
                Language = "en-US",
                MovieLibraries = new List<LibraryContainer>(),
                ShowLibraries = new List<LibraryContainer>(),
                ToShortMovie = 10,
                UpdateInProgress = false,
                UpdateTrain = UpdateTrain.Beta,
                WizardFinished = true,
                MediaServer = new MediaServerSettings
                {
                    ApiKey = "1234567980",
                    ServerName = "ServerName",
                    AuthorizationScheme = "MediaBrowser",
                    ServerAddress = "localhost",
                    ServerPort = 8097,
                    ServerProtocol = ConnectionProtocol.Https,
                },
                Tmdb = new TmdbSettings
                {
                    LastUpdate = new DateTime(2019, 1, 1),
                    ApiKey = "ABCDE"
                }
            };

            _appSettings = new AppSettings
            {
                Version = "0.0.0.0",
                Dirs = new Dirs { Data = "data-dir", Logs = "log-dir", Config = "config-dir"}
            };

            _settingsServiceMock = new Mock<ISettingsService>();
            _settingsServiceMock.Setup(x => x.GetAppSettings()).Returns(_appSettings);
            _settingsServiceMock.Setup(x => x.GetUserSettings()).Returns(_userSettings);
            _settingsServiceMock.Setup(x => x.SaveUserSettingsAsync(It.IsAny<UserSettings>()))
                .Returns(Task.FromResult(_userSettings));

            var useSettings = new FullSettingsViewModelBuilder(_userSettings)
                .AddMovieLibraries(_userSettings.MovieLibraries)
                .AddShowLibraries(_userSettings.ShowLibraries)
                .Build();
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(x => x.Map<FullSettingsViewModel>(It.IsAny<UserSettings>()))
                .Returns(useSettings);
            mapperMock.Setup(x => x.Map<UserSettings>(It.IsAny<FullSettingsViewModel>()))
                .Returns(new UserSettings
                {
                    MovieLibraries = new List<LibraryContainer>(),
                    ShowLibraries = new List<LibraryContainer>()
                });

            var statisticsRepositoryMock = new Mock<IStatisticsRepository>();
            statisticsRepositoryMock.Setup(x => x.MarkMovieTypesAsInvalid());
            statisticsRepositoryMock.Setup(x => x.MarkShowTypesAsInvalid());

            var languageServiceMock = new Mock<ILanguageService>();
            languageServiceMock.Setup(x => x.GetLanguages()).Returns(new List<Language>
            {
                new Language {Code = "BE", Id = "1", Name = "Dutch"},
                new Language {Code = "EN", Id = "2", Name = "English"}
            });

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
            var resultObject = result.Should().BeOfType<OkObjectResult>().Subject.Value;
            var settings = resultObject.Should().BeOfType<FullSettingsViewModel>().Subject;

            settings.AppName.Should().Be(_userSettings.AppName);
            settings.AutoUpdate.Should().Be(_userSettings.AutoUpdate);
            settings.EnableRollbarLogging.Should().Be(_userSettings.EnableRollbarLogging);
            settings.Id.Should().Be(_userSettings.Id);
            settings.KeepLogsCount.Should().Be(_userSettings.KeepLogsCount);
            settings.Language.Should().Be(_userSettings.Language);
            settings.NoUpdates.Should().Be(_appSettings.NoUpdates);
            settings.ToShortMovie.Should().Be(_userSettings.ToShortMovie);
            settings.ToShortMovieEnabled.Should().Be(_userSettings.ToShortMovieEnabled);
            settings.UpdateInProgress.Should().Be(_userSettings.UpdateInProgress);
            settings.UpdateTrain.Should().Be((int)_userSettings.UpdateTrain);
            settings.WizardFinished.Should().Be(_userSettings.WizardFinished);
            settings.MovieLibraries.Count.Should().Be(_userSettings.MovieLibraries.Count);
            settings.ShowLibraries.Count.Should().Be(_userSettings.ShowLibraries.Count);
            settings.MediaServer.ApiKey.Should().Be(_userSettings.MediaServer.ApiKey);
            settings.MediaServer.AuthorizationScheme.Should().Be(_userSettings.MediaServer.AuthorizationScheme);
            settings.MediaServer.ServerAddress.Should().Be(_userSettings.MediaServer.ServerAddress);
            settings.MediaServer.ServerName.Should().Be(_userSettings.MediaServer.ServerName);
            settings.MediaServer.ServerPort.Should().Be(_userSettings.MediaServer.ServerPort);
            settings.MediaServer.ServerProtocol.Should().Be((int)_userSettings.MediaServer.ServerProtocol);


            settings.DataDir.Should().Be(_appSettings.Dirs.Data);
            settings.LogDir.Should().Be(_appSettings.Dirs.Logs);
            settings.ConfigDir.Should().Be(_appSettings.Dirs.Config);
            settings.Version.Should().Be(_appSettings.Version);

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
                MovieLibraries = new List<LibraryContainerViewModel>(),
                ShowLibraries = new List<LibraryContainerViewModel>(),
                ToShortMovie = 10,
                UpdateInProgress = false,
                UpdateTrain = 0,
                WizardFinished = true,
                MediaServer = new MediaServerSettingsViewModel
                {
                    ApiKey = "1234567980",
                    ServerName = "ServerName",
                    AuthorizationScheme = "MediaBrowser",
                    ServerAddress = "localhost",
                    ServerPort = 8097,
                    ServerProtocol = 1,
                },
                Tmdb = new TmdbSettingsViewModel
                {
                    LastUpdate = new DateTime(2019, 1, 1),
                    ApiKey = "ABCDE"
                }
            };

            await _subject.Update(settings);

            _settingsServiceMock.Verify(x => x.SaveUserSettingsAsync(It.IsAny<UserSettings>()), Times.Once);
        }

        [Fact]
        public void GetLanguages_Should_Return_All_Languages()
        {
            var result = _subject.GetList();
            result.Should().BeOfType<OkObjectResult>();
        }
    }
}
