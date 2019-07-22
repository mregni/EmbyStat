using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Services;
using EmbyStat.Services.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace Tests.Unit.Services
{
    [Collection("Mapper collection")]
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
            if (Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Logs-test1")))
            {
                Directory.Delete(Path.Combine(Directory.GetCurrentDirectory(), "Logs-test1"), true);
            }

            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Logs-test1"));

            var settings = new AppSettings { Dirs = new Dirs { Logs = "Logs-test1" } };
            var userSettings = new UserSettings { KeepLogsCount = 10 };

            _settingsServiceMock.Setup(x => x.GetAppSettings()).Returns(settings);
            _settingsServiceMock.Setup(x => x.GetUserSettings()).Returns(userSettings);

            var service = new LogService(_settingsServiceMock.Object);

            File.Create(Path.Combine(Directory.GetCurrentDirectory(), "Logs-test1", "log1.txt"));
            Thread.Sleep(1001);
            File.Create(Path.Combine(Directory.GetCurrentDirectory(), "Logs-test1", "log2.txt"));

            var list = service.GetLogFileList();
            list.Should().NotBeNull();
            list.Count.Should().Be(2);
            list[0].FileName.Should().Be("log2");
            list[1].FileName.Should().Be("log1");
        }

        [Fact]
        public void GetLimitedLogList()
        {
            if (Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Logs-test2")))
            {
                Directory.Delete(Path.Combine(Directory.GetCurrentDirectory(), "Logs-test2"), true);
            }

            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Logs-test2"));

            var settings = new AppSettings { Dirs = new Dirs { Logs = "Logs-test2" } };
            _settingsServiceMock.Setup(x => x.GetAppSettings()).Returns(settings);

            var userSettings = new UserSettings();
            userSettings.KeepLogsCount = 1;

            _settingsServiceMock.Setup(x => x.GetUserSettings()).Returns(userSettings);
            var service = new LogService(_settingsServiceMock.Object);

            File.Create(Path.Combine(Directory.GetCurrentDirectory(), "Logs-test2", "log1.txt"));
            Thread.Sleep(1001);
            File.Create(Path.Combine(Directory.GetCurrentDirectory(), "Logs-test2", "log2.txt"));

            var list = service.GetLogFileList();
            list.Should().NotBeNull();
            list.Count.Should().Be(1);
            list[0].FileName.Should().Be("log2");
        }

        [Fact]
        public void GetLogStream()
        {
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Logs-test3"));

            var settings = new AppSettings { Dirs = new Dirs { Logs = "Logs-test3" } };
            var userSettings = new UserSettings { Emby = new EmbySettings { UserName = "reggi", ServerProtocol = ConnectionProtocol.Http, ServerAddress = "192.168.1.1", ServerPort = 8001 }, Tvdb = new TvdbSettings { ApiKey = "0000" } };
            _settingsServiceMock.Setup(x => x.GetAppSettings()).Returns(settings);
            _settingsServiceMock.Setup(x => x.GetUserSettings()).Returns(userSettings);
            var service = new LogService(_settingsServiceMock.Object);

            var line = "Log line: http://192.168.1.1:8001; ApiKey:0000; username:reggi";
            File.AppendAllText(Path.Combine(Directory.GetCurrentDirectory(), "Logs-test3", "log1.txt"), line);

            var stream = service.GetLogStream("log1.txt", false);
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                var lines = reader.ReadToEnd();
                lines.Should().Be(line);
            }

            Directory.Delete(Path.Combine(Directory.GetCurrentDirectory(), "Logs-test3"), true);
        }

        [Fact]
        public void GetAnonymousLogStream()
        {
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Logs-test4"));

            var settings = new AppSettings { Dirs = new Dirs { Logs = "Logs-test4" } };
            var userSettings = new UserSettings { Emby = new EmbySettings { UserName = "reggi", ServerProtocol = ConnectionProtocol.Http, ServerAddress = "192.168.1.1", ServerPort = 8001 }, Tvdb = new TvdbSettings { ApiKey = "0000" } };
            _settingsServiceMock.Setup(x => x.GetAppSettings()).Returns(settings);
            _settingsServiceMock.Setup(x => x.GetUserSettings()).Returns(userSettings);
            var service = new LogService(_settingsServiceMock.Object);

            var line = "Log line: http://192.168.1.1:8001; ApiKey:0000; username:reggi\r\n";
            File.AppendAllText(Path.Combine(Directory.GetCurrentDirectory(), "Logs-test4", "log1.txt"), line);

            var stream = service.GetLogStream("log1.txt", true);
            using (var reader = new StreamReader(stream))
            {
                var lines = reader.ReadToEnd();
                line = line.Replace(userSettings.FullEmbyServerAddress, "http://xxx.xxx.xxx.xxx:xxxx");
                line = line.Replace(userSettings.Tvdb.ApiKey, "xxxxxxxxxxxxxx");
                line = line.Replace(userSettings.Emby.UserName, "{EMBY ADMIN USER}");
                lines.Should().Be(line);
            }

            Directory.Delete(Path.Combine(Directory.GetCurrentDirectory(), "Logs-test4"), true);
        }
    }
}
