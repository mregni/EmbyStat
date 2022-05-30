// using System;
// using System.IO;
// using EmbyStat.Configuration;
// using FluentAssertions;
// using Microsoft.Extensions.Options;
// using Moq;
// using Newtonsoft.Json;
// using Tests.Unit.Builders;
// using Xunit;
//
// namespace Tests.Unit.Services;
//
// public class SettingsServiceTests
// {
//     private readonly ConfigurationService _subject;
//     private Guid DeviceId { get; }
//
//     public SettingsServiceTests()
//     {
//         DeviceId = Guid.NewGuid();
//
//         var config = new ConfigBuilder().Build();
//         var appSettingsMock = new Mock<IOptions<Config>>();
//         appSettingsMock.Setup(x => x.Value).Returns(config);
//     
//         _subject = new ConfigurationService(appSettingsMock.Object);
//     }
//
//     private void SetupSettingsFile()
//     {
//         var fileSettings = new ConfigBuilder()
//             .WithDeviceId(DeviceId)
//             .WithLogCount(10)
//             .WithAutoUpdate(false)
//             .WithLanguage("en-US")
//             .WithMigration(3)
//             .WithAppName("EmbyStat")
//             .Build();
//
//         var strJson = JsonConvert.SerializeObject(fileSettings, Formatting.Indented);
//         Directory.CreateDirectory("config");
//         var dir = Path.Combine("config", "usersettings.json");
//         File.WriteAllText(dir, strJson);
//     }
//
//     [Fact]
//     public async void SaveUserSettings()
//     {
//         SetupSettingsFile();
//         var config = new ConfigBuilder()
//             .WithDeviceId(DeviceId)
//             .WithLogCount(10)
//             .WithAutoUpdate(false)
//             .WithLanguage("en-US")
//             .WithMigration(3)
//             .WithAppName("EmbyStat")
//             .WithMediaServerAddress("https://localhost:8000")
//             .Build();
//
//         _subject.LoadUserSettingsFromFile();
//         await _subject.SaveUserSettingsAsync(settings);
//
//         var settingsFilePath = Path.Combine("config", "usersettings.json");
//         File.Exists(settingsFilePath).Should().BeTrue();
//         var loadedSettings = JsonConvert.DeserializeObject<UserSettings>(await File.ReadAllTextAsync(settingsFilePath));
//
//         loadedSettings.Should().NotBeNull();
//         loadedSettings!.Id.Should().Be(DeviceId);
//         loadedSettings.AppName.Should().Be("EmbyStat");
//         loadedSettings.AutoUpdate.Should().BeFalse();
//         loadedSettings.KeepLogsCount.Should().Be(10);
//         loadedSettings.Language.Should().Be("en-US");
//     }
//
//     [Fact]
//     public void GetUserSettings()
//     {
//         SetupSettingsFile();
//         _subject.LoadUserSettingsFromFile();
//         var settings = _subject.GetUserSettings();
//         settings.Id.Should().Be(DeviceId);
//         settings.AppName.Should().Be("EmbyStat");
//         settings.AutoUpdate.Should().BeFalse();
//         settings.KeepLogsCount.Should().Be(10);
//         settings.Language.Should().Be("en-US");
//     }
//
//     [Fact]
//     public void GetAppSettings_Should_Return_App_Settings()
//     {
//         var settings = _subject.GetAppSettings();
//         settings.Should().NotBeNull();
//
//         settings.Version.Should().Be("0.0.0.0");
//         settings.Dirs.Config.Should().Be("config");
//     }
//
//     [Fact]
//     public void GetUserSettingsVersionWithSettingsFile()
//     {
//         SetupSettingsFile();
//         _subject.LoadUserSettingsFromFile();
//         var version = _subject.GetUserSettingsVersion();
//         version.Should().Be(3);
//     }
//
//     [Fact]
//     public void GetUserSettingsVersionWithoutSettingsFile()
//     {
//         var version = _subject.GetUserSettingsVersion();
//         version.Should().Be(0);
//     }
// }