using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Configuration.Interfaces;
using EmbyStat.Core.Logging;
using EmbyStat.Core.MediaServers.Interfaces;
using FluentAssertions;
using Moq;
using Tests.Unit.Builders;
using Xunit;

namespace Tests.Unit.Services;

public class LogServiceTests
{
    private readonly Mock<IConfigurationService> _configurationServiceMock;

    public LogServiceTests()
    {
        _configurationServiceMock = new Mock<IConfigurationService>();
    }

    [Fact]
    public void GetLogList()
    {
        var logPath = Path.Combine("log-dir", "Logs-test1");
        if (Directory.Exists(logPath.GetLocalPath()))
        {
            Directory.Delete(logPath.GetLocalPath(), true);
        }

        Directory.CreateDirectory(logPath.GetLocalPath());

        var config = new ConfigBuilder()
            .WithLogsDir(logPath)
            .Build();

        _configurationServiceMock.Setup(x => x.Get()).Returns(config);

        var embyServiceMock = new Mock<IMediaServerService>();

        var service = new LogService(_configurationServiceMock.Object, embyServiceMock.Object);

        File.Create(Path.Combine("log-dir", "Logs-test1", "log1.txt").GetLocalPath());
        Thread.Sleep(1001);
        File.Create(Path.Combine("log-dir", "Logs-test1", "log2.txt").GetLocalPath());

        var list = service.GetLogFileList().ToList();
        list.Should().NotBeNull();
        list.Count.Should().Be(2);
        list[0].Name.Should().Be("log2.txt");
        list[1].Name.Should().Be("log1.txt");
    }

    [Fact]
    public void GetLimitedLogList()
    {
        var logPath = Path.Combine("log-dir", "Logs-test2");
        if (Directory.Exists(logPath.GetLocalPath()))
        {
            Directory.Delete(logPath.GetLocalPath(), true);
        }

        Directory.CreateDirectory(logPath.GetLocalPath());
        
        var config = new ConfigBuilder()
            .WithLogsDir(logPath)
            .WithLogCount(1)
            .Build();

        _configurationServiceMock.Setup(x => x.Get()).Returns(config);

        var embyServiceMock = new Mock<IMediaServerService>();
        var service = new LogService(_configurationServiceMock.Object, embyServiceMock.Object);

        File.Create(Path.Combine("log-dir", "Logs-test2", "log1.txt").GetLocalPath());
        Thread.Sleep(1001);
        File.Create(Path.Combine("log-dir", "Logs-test2", "log2.txt").GetLocalPath());

        var list = service.GetLogFileList().ToList();
        list.Should().NotBeNull();
        list.Count.Should().Be(1);
        list[0].Name.Should().Be("log2.txt");
    }

    [Fact]
    public async Task GetLogStream_Should_Return_Log()
    {
        var logPath = Path.Combine("log-dir", "Logs-test3");
        if (Directory.Exists(logPath.GetLocalPath()))
        {
            Directory.Delete(logPath.GetLocalPath(), true);
        }

        Directory.CreateDirectory(logPath.GetLocalPath());

        var config = new ConfigBuilder()
            .WithLogsDir(logPath)
            .WithMediaServerAddress("http://192.168.1.1:8001")
            .WithTmdbApiKey("0000")
            .Build();
        
        _configurationServiceMock.Setup(x => x.Get()).Returns(config);

        var embyServiceMock = new Mock<IMediaServerService>();
        embyServiceMock
            .Setup(x => x.GetServerInfo(false))
            .ReturnsAsync(new MediaServerInfo {Id = Guid.NewGuid().ToString()});

        var service = new LogService(_configurationServiceMock.Object, embyServiceMock.Object);

        const string line = "Log line: http://192.168.1.1:8001; ApiKey:0000; username:reggi";
        await File.AppendAllTextAsync(Path.Combine("log-dir", "Logs-test3", "log1.txt").GetLocalPath(), line);

        var stream = await service.GetLogStream("log1.txt", false);
        stream.Position = 0;

        using var reader = new StreamReader(stream);
        var lines = await reader.ReadToEndAsync();
        lines.Should().Be(line);
    }

    [Fact]
    public async Task GetAnonymousLogStream_Should_Return_Log()
    {
        var logPath = Path.Combine("log-dir", "Logs-test4");
        if (Directory.Exists(logPath.GetLocalPath()))
        {
            Directory.Delete(logPath.GetLocalPath(), true);
        }

        Directory.CreateDirectory(logPath.GetLocalPath());

        var config = new ConfigBuilder()
            .WithLogsDir(logPath)
            .WithMediaServerAddress("http://192.168.1.1:8001")
            .WithMediaServerApiKey("123456")
            .WithTmdbApiKey("0000")
            .Build();

        _configurationServiceMock.Setup(x => x.Get()).Returns(config);

        var embyServiceMock = new Mock<IMediaServerService>();
        embyServiceMock
            .Setup(x => x.GetServerInfo(false))
            .ReturnsAsync(new MediaServerInfo {Id = "654321"});

        var service = new LogService(_configurationServiceMock.Object, embyServiceMock.Object);

        const string line =
            "Log line: http://192.168.1.1:8001; ApiKey:0000; ws://192.168.1.1:8001; serverId:654321\r\n";
        const string anonymousLine =
            "Log line: https://xxx.xxx.xxx.xxx:xxxx; ApiKey:xxxxxxxxxxxxxx; wss://xxx.xxx.xxx.xxx:xxxx; serverId:xxxxxxxxxxxxxx\r\n";
        await File.AppendAllTextAsync(Path.Combine("log-dir", "Logs-test4", "log1.txt").GetLocalPath(), line);

        var stream = await service.GetLogStream("log1.txt", true);
        stream.Position = 0;

        using var reader = new StreamReader(stream);
        var lines = await reader.ReadToEndAsync();
        lines.Should().Be(anonymousLine);
    }
}