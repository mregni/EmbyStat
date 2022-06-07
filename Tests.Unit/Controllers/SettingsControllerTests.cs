using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Configuration;
using EmbyStat.Configuration.Interfaces;
using EmbyStat.Controllers.Settings;
using EmbyStat.Core.Languages.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Tests.Unit.Builders;
using Tests.Unit.Builders.ViewModels;
using Xunit;
using Rollbar = EmbyStat.Configuration.Rollbar;

namespace Tests.Unit.Controllers;

public class SettingsControllerTests : IDisposable
{
    private readonly SettingsController _subject;
    private readonly Config _config;
    private readonly Mock<IConfigurationService> _configurationServiceMock;
    private readonly Mock<ILanguageService> _languageServiceMock;

    private Guid DeviceId { get; }

    public SettingsControllerTests()
    {
        DeviceId = Guid.NewGuid();
        _config = new ConfigBuilder()
            .WithDeviceId(DeviceId)
            .Build();

        _configurationServiceMock = new Mock<IConfigurationService>();
        _configurationServiceMock.Setup(x => x.Get()).Returns(_config);

        var useSettings = new ConfigViewModelBuilder(_config).Build();
        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(x => x.Map<ConfigViewModel>(It.IsAny<Config>()))
            .Returns(useSettings);

        _languageServiceMock = new Mock<ILanguageService>();
        _languageServiceMock.Setup(x => x.GetLanguages()).ReturnsAsync(new List<Language>
        {
            new() {Code = "BE", Id = "1", Name = "Dutch"},
            new() {Code = "EN", Id = "2", Name = "English"}
        });
        
        var logger = new Mock<ILogger<SettingsController>>();
        _subject = new SettingsController(_configurationServiceMock.Object, _languageServiceMock.Object, mapperMock.Object, logger.Object);
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
        var settings = resultObject.Should().BeOfType<ConfigViewModel>().Subject;

        settings.SystemConfig.AppName.Should().Be(_config.SystemConfig.AppName);
        settings.SystemConfig.AutoUpdate.Should().Be(_config.SystemConfig.AutoUpdate);
        settings.UserConfig.EnableRollbarLogging.Should().Be(_config.UserConfig.EnableRollbarLogging);
        settings.SystemConfig.Id.Should().Be(_config.SystemConfig.Id);
        settings.UserConfig.KeepLogsCount.Should().Be(_config.UserConfig.KeepLogsCount);
        settings.UserConfig.Language.Should().Be(_config.UserConfig.Language);
        settings.SystemConfig.CanUpdate.Should().Be(_config.SystemConfig.CanUpdate);
        settings.UserConfig.ToShortMovie.Should().Be(_config.UserConfig.ToShortMovie);
        settings.UserConfig.ToShortMovieEnabled.Should().Be(_config.UserConfig.ToShortMovieEnabled);
        settings.SystemConfig.UpdateInProgress.Should().Be(_config.SystemConfig.UpdateInProgress);
        settings.SystemConfig.UpdateTrain.Should().Be((int) _config.SystemConfig.UpdateTrain);
        settings.UserConfig.WizardFinished.Should().Be(_config.UserConfig.WizardFinished);
        settings.UserConfig.MediaServer.ApiKey.Should().Be(_config.UserConfig.MediaServer.ApiKey);
        settings.UserConfig.MediaServer.AuthorizationScheme.Should().Be(_config.UserConfig.MediaServer.AuthorizationScheme);
        settings.UserConfig.MediaServer.Address.Should().Be(_config.UserConfig.MediaServer.Address);

        settings.SystemConfig.Dirs.Data.Should().Be(_config.SystemConfig.Dirs.Data);
        settings.SystemConfig.Dirs.Logs.Should().Be(_config.SystemConfig.Dirs.Logs);
        settings.SystemConfig.Dirs.Updater.Should().Be(_config.SystemConfig.Dirs.Updater);
        settings.SystemConfig.Dirs.TempUpdate.Should().Be(_config.SystemConfig.Dirs.TempUpdate);
        settings.SystemConfig.Version.Should().Be(_config.SystemConfig.Version);

        _configurationServiceMock.Verify(x => x.Get(), Times.Once);
        _configurationServiceMock.VerifyNoOtherCalls();
        _languageServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async void IsConfigurationUpdatedCorrectly()
    {
        var config = new ConfigBuilder().Build();
        var viewModel = new ConfigViewModelBuilder(config).Build();

        await _subject.Update(viewModel.UserConfig);

        _configurationServiceMock.Verify(x => x.UpdateUserConfiguration(It.IsAny<UserConfig>()), Times.Once);
        _configurationServiceMock.Verify(x => x.Get(), Times.Once);
        _configurationServiceMock.VerifyNoOtherCalls();
        _languageServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetLanguages_Should_Return_All_Languages()
    {
        var result = await _subject.GetList();
        result.Should().BeOfType<OkObjectResult>();
            
        _languageServiceMock.Verify(x => x.GetLanguages(), Times.Once);
        _configurationServiceMock.VerifyNoOtherCalls();
        _languageServiceMock.VerifyNoOtherCalls();
    }
}