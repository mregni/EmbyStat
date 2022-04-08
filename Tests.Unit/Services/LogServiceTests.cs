using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Services;
using EmbyStat.Services.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace Tests.Unit.Services
{
    public class LogServiceTests
    {
        private readonly Mock<ISettingsService> _settingsServiceMock;

        public LogServiceTests()
        {
            _settingsServiceMock = new Mock<ISettingsService>();
        }

        [Fact]
        public void GetLogList()
        {
            if (Directory.Exists(Path.Combine("config", "Logs-test1").GetLocalPath()))
            {
                Directory.Delete(Path.Combine("config", "Logs-test1").GetLocalPath(), true);
            }

            Directory.CreateDirectory(Path.Combine("config", "Logs-test1").GetLocalPath());

            var settings = new AppSettings {Dirs = new Dirs {Logs = "Logs-test1", Config = "config"}};
            var userSettings = new UserSettings {KeepLogsCount = 10};

            _settingsServiceMock.Setup(x => x.GetAppSettings()).Returns(settings);
            _settingsServiceMock.Setup(x => x.GetUserSettings()).Returns(userSettings);

            var embyServiceMock = new Mock<IMediaServerService>();

            var service = new LogService(_settingsServiceMock.Object, embyServiceMock.Object);

            File.Create(Path.Combine("config", "Logs-test1", "log1.txt").GetLocalPath());
            Thread.Sleep(1001);
            File.Create(Path.Combine("config", "Logs-test1", "log2.txt").GetLocalPath());

            var list = service.GetLogFileList();
            list.Should().NotBeNull();
            list.Count.Should().Be(2);
            list[0].FileName.Should().Be("log2.txt");
            list[1].FileName.Should().Be("log1.txt");
        }

        [Fact]
        public void GetLimitedLogList()
        {
            if (Directory.Exists(Path.Combine("config", "Logs-test2").GetLocalPath()))
            {
                Directory.Delete(Path.Combine("config", "Logs-test2").GetLocalPath(), true);
            }

            Directory.CreateDirectory(Path.Combine("config", "Logs-test2").GetLocalPath());

            var settings = new AppSettings {Dirs = new Dirs {Logs = "Logs-test2", Config = "config"}};
            _settingsServiceMock.Setup(x => x.GetAppSettings()).Returns(settings);

            var userSettings = new UserSettings {KeepLogsCount = 1};

            _settingsServiceMock.Setup(x => x.GetUserSettings()).Returns(userSettings);
            var embyServiceMock = new Mock<IMediaServerService>();

            var service = new LogService(_settingsServiceMock.Object, embyServiceMock.Object);

            File.Create(Path.Combine("config", "Logs-test2", "log1.txt").GetLocalPath());
            Thread.Sleep(1001);
            File.Create(Path.Combine("config", "Logs-test2", "log2.txt").GetLocalPath());

            var list = service.GetLogFileList();
            list.Should().NotBeNull();
            list.Count.Should().Be(1);
            list[0].FileName.Should().Be("log2.txt");
        }

        [Fact]
        public async Task GetLogStream_Should_Return_Log()
        {
            if (Directory.Exists(Path.Combine("config", "Logs-test3").GetLocalPath()))
            {
                Directory.Delete(Path.Combine("config", "Logs-test3").GetLocalPath(), true);
            }

            Directory.CreateDirectory(Path.Combine("config", "Logs-test3").GetLocalPath());

            var settings = new AppSettings {Dirs = new Dirs {Logs = "Logs-test3", Config = "config"}};
            var userSettings = new UserSettings
            {
                MediaServer = new MediaServerSettings
                    {Address = "http://192.168.1.1:8001"},
                Tmdb = new TmdbSettings {ApiKey = "0000"}
            };
            _settingsServiceMock.Setup(x => x.GetAppSettings()).Returns(settings);
            _settingsServiceMock.Setup(x => x.GetUserSettings()).Returns(userSettings);

            var embyServiceMock = new Mock<IMediaServerService>();
            embyServiceMock
                .Setup(x => x.GetServerInfo(false))
                .ReturnsAsync(new MediaServerInfo {Id = Guid.NewGuid().ToString()});

            var service = new LogService(_settingsServiceMock.Object, embyServiceMock.Object);

            var line = "Log line: http://192.168.1.1:8001; ApiKey:0000; username:reggi";
            await File.AppendAllTextAsync(Path.Combine("config", "Logs-test3", "log1.txt").GetLocalPath(), line);

            var stream = await service.GetLogStream("log1.txt", false);
            stream.Position = 0;

            using var reader = new StreamReader(stream);
            var lines = await reader.ReadToEndAsync();
            lines.Should().Be(line);
        }

        [Fact]
        public async Task GetAnonymousLogStream_Should_Return_Log()
        {
            if (Directory.Exists(Path.Combine("config", "Logs-test4").GetLocalPath()))
            {
                Directory.Delete(Path.Combine("config", "Logs-test4").GetLocalPath(), true);
            }

            Directory.CreateDirectory(Path.Combine("config", "Logs-test4").GetLocalPath());

            var settings = new AppSettings {Dirs = new Dirs {Logs = "Logs-test4", Config = "config"}};
            var userSettings = new UserSettings
            {
                MediaServer = new MediaServerSettings
                {
                    Address = "http://192.168.1.1:8001",
                    ApiKey = "123456"
                },
                Tmdb = new TmdbSettings {ApiKey = "0000"}
            };
            _settingsServiceMock.Setup(x => x.GetAppSettings()).Returns(settings);
            _settingsServiceMock.Setup(x => x.GetUserSettings()).Returns(userSettings);

            var embyServiceMock = new Mock<IMediaServerService>();
            embyServiceMock
                .Setup(x => x.GetServerInfo(false))
                .ReturnsAsync(new MediaServerInfo {Id = "654321"});

            var service = new LogService(_settingsServiceMock.Object, embyServiceMock.Object);

            const string line =
                "Log line: http://192.168.1.1:8001; ApiKey:0000; ws://192.168.1.1:8001; serverId:654321\r\n";
            const string anonymousLine =
                "Log line: https://xxx.xxx.xxx.xxx:xxxx; ApiKey:xxxxxxxxxxxxxx; wss://xxx.xxx.xxx.xxx:xxxx; serverId:xxxxxxxxxxxxxx\r\n";
            await File.AppendAllTextAsync(Path.Combine("config", "Logs-test4", "log1.txt").GetLocalPath(), line);

            var stream = await service.GetLogStream("log1.txt", true);
            stream.Position = 0;

            using var reader = new StreamReader(stream);
            var lines = await reader.ReadToEndAsync();
            lines.Should().Be(anonymousLine);
        }
    }
}