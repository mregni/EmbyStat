using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Controllers.Settings;
using EmbyStat.Core.Languages.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Tests.Unit.Builders.ViewModels;
using Xunit;

namespace Tests.Unit.Controllers;

public class SettingsControllerTests : IDisposable
{
    private readonly SettingsController _subject;
    private readonly AppSettings _appSettings;
    private readonly UserSettings _userSettings;
    private readonly Mock<IRollbarService> _settingsServiceMock;
    private readonly Mock<ILanguageService> _languageServiceMock;

    private Guid DeviceId { get; }

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
            ToShortMovie = 10,
            UpdateInProgress = false,
            UpdateTrain = UpdateTrain.Beta,
            WizardFinished = true,
            MediaServer = new MediaServerSettings
            {
                ApiKey = "1234567980",
                Name = "ServerName",
                AuthorizationScheme = "MediaBrowser",
                Address = "https://localhost:8097",
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
            Dirs = new Dirs {Data = "data-dir", Logs = "log-dir", Config = "config-dir"}
        };

        _settingsServiceMock = new Mock<IRollbarService>();
        _settingsServiceMock.Setup(x => x.GetAppSettings()).Returns(_appSettings);
        _settingsServiceMock.Setup(x => x.GetUserSettings()).Returns(_userSettings);
        _settingsServiceMock.Setup(x => x.SaveUserSettingsAsync(It.IsAny<UserSettings>()))
            .Returns(Task.FromResult(_userSettings));

        var useSettings = new FullSettingsViewModelBuilder(_userSettings).Build();
        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(x => x.Map<FullSettingsViewModel>(It.IsAny<UserSettings>()))
            .Returns(useSettings);
        mapperMock.Setup(x => x.Map<UserSettings>(It.IsAny<FullSettingsViewModel>()))
            .Returns(new UserSettings());

        _languageServiceMock = new Mock<ILanguageService>();
        _languageServiceMock.Setup(x => x.GetLanguages()).ReturnsAsync(new List<Language>
        {
            new() {Code = "BE", Id = "1", Name = "Dutch"},
            new() {Code = "EN", Id = "2", Name = "English"}
        });
        
        var logger = new Mock<ILogger<SettingsController>>();
        _subject = new SettingsController(_settingsServiceMock.Object, _languageServiceMock.Object, mapperMock.Object, logger.Object);
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
        settings.UpdateTrain.Should().Be((int) _userSettings.UpdateTrain);
        settings.WizardFinished.Should().Be(_userSettings.WizardFinished);
        settings.MediaServer.ApiKey.Should().Be(_userSettings.MediaServer.ApiKey);
        settings.MediaServer.AuthorizationScheme.Should().Be(_userSettings.MediaServer.AuthorizationScheme);
        settings.MediaServer.Address.Should().Be(_userSettings.MediaServer.Address);

        settings.DataDir.Should().Be(_appSettings.Dirs.Data);
        settings.LogDir.Should().Be(_appSettings.Dirs.Logs);
        settings.ConfigDir.Should().Be(_appSettings.Dirs.Config);
        settings.Version.Should().Be(_appSettings.Version);

        _settingsServiceMock.Verify(x => x.GetUserSettings(), Times.Once);
        _settingsServiceMock.Verify(x => x.GetAppSettings(), Times.Once);
        _settingsServiceMock.VerifyNoOtherCalls();
        _languageServiceMock.VerifyNoOtherCalls();
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
            ToShortMovie = 10,
            UpdateInProgress = false,
            UpdateTrain = 0,
            WizardFinished = true,
            MediaServer = new MediaServerSettingsViewModel
            {
                ApiKey = "1234567980",
                Name = "ServerName",
                AuthorizationScheme = "MediaBrowser",
                Address = "https://localhost:8097",
            },
            Tmdb = new TmdbSettingsViewModel
            {
                LastUpdate = new DateTime(2019, 1, 1),
                ApiKey = "ABCDE"
            }
        };

        await _subject.Update(settings);

        _settingsServiceMock.Verify(x => x.SaveUserSettingsAsync(It.IsAny<UserSettings>()), Times.Once);
        _settingsServiceMock.Verify(x => x.GetAppSettings(), Times.Once);
        _settingsServiceMock.VerifyNoOtherCalls();
        _languageServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetLanguages_Should_Return_All_Languages()
    {
        var result = await _subject.GetList();
        result.Should().BeOfType<OkObjectResult>();
            
        _languageServiceMock.Verify(x => x.GetLanguages(), Times.Once);
        _settingsServiceMock.VerifyNoOtherCalls();
        _languageServiceMock.VerifyNoOtherCalls();
    }
}