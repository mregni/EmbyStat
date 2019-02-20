using System;
using System.Collections.Generic;
using System.IO;
using Castle.Components.DictionaryAdapter;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Tests.Unit.Services
{
    [Collection("Services collection")]
    public class SettingsServiceTests
    {
        private readonly SettingsService _subject;
        private Guid DeviceId { get; set; }
        public SettingsServiceTests()
        {
            DeviceId = Guid.NewGuid();
            SetupSettingsFile();

            var appSettingsMock = new Mock<IOptions<AppSettings>>();
            appSettingsMock.Setup(x => x.Value).Returns(new AppSettings { Version = "0.0.0.0", Dirs = new Dirs() { Settings = "Settings" } });

            var statisticsRepositoryMock = new Mock<IStatisticsRepository>();
            statisticsRepositoryMock.Setup(x => x.MarkMovieTypesAsInvalid());
            statisticsRepositoryMock.Setup(x => x.MarkShowTypesAsInvalid());

            _subject = new SettingsService(appSettingsMock.Object, statisticsRepositoryMock.Object);
        }

        private void SetupSettingsFile()
        {
            var fileSettings = new UserSettings
            {
                Id = DeviceId,
                AppName = "EmbyStat",
                AutoUpdate = false,
                KeepLogsCount = 10,
                Language = "en-US",
                MovieCollectionTypes = new EditableList<CollectionType>(),
                ShowCollectionTypes = new List<CollectionType>()
            };

            var strJson = JsonConvert.SerializeObject(fileSettings, Formatting.Indented);
            Directory.CreateDirectory("Settings");
            var dir = Path.Combine("Settings", "usersettings.json");
            File.WriteAllText(dir, strJson);
        }

        [Fact]
        public async void SaveUserSettings()
        {
            var settings = new UserSettings
            {
                Id = DeviceId,
                AppName = "EmbyStat",
                AutoUpdate = false,
                KeepLogsCount = 10,
                Language = "en-US",
                MovieCollectionTypes = new EditableList<CollectionType>(),
                ShowCollectionTypes = new List<CollectionType>()
            };

            await _subject.SaveUserSettings(settings);

            var settingsFilePath = Path.Combine("Settings", "usersettings.json");
            File.Exists(settingsFilePath).Should().BeTrue();
            var loadedSettings = JsonConvert.DeserializeObject<UserSettings>(File.ReadAllText(settingsFilePath));

            loadedSettings.Id.Should().Be(DeviceId);
            loadedSettings.AppName.Should().Be("EmbyStat");
            loadedSettings.AutoUpdate.Should().BeFalse();
            loadedSettings.KeepLogsCount.Should().Be(10);
            loadedSettings.Language.Should().Be("en-US");
        }

        [Fact]
        public void GetUserSettings()
        {
            var settings = _subject.GetUserSettings();
            settings.Id.Should().Be(DeviceId);
            settings.AppName.Should().Be("EmbyStat");
            settings.AutoUpdate.Should().BeFalse();
            settings.KeepLogsCount.Should().Be(10);
            settings.Language.Should().Be("en-US");
        }
    }
}
