using EmbyStat.Common.Extensions;
using EmbyStat.Configuration.Interfaces;
using EmbyStat.Core.Logging.Interfaces;
using EmbyStat.Core.MediaServers.Interfaces;

namespace EmbyStat.Core.Logging;

public class LogService : ILogService
{
    private readonly IConfigurationService _configurationService;
    private readonly IMediaServerService _mediaServerService;

    public LogService(IConfigurationService configurationService, IMediaServerService mediaServerService)
    {
        _configurationService = configurationService;
        _mediaServerService = mediaServerService;
    }

    public IEnumerable<LogFile> GetLogFileList()
    {
        var config = _configurationService.Get();
        var list = new List<LogFile>();
        foreach (var filePath in Directory.EnumerateFiles(config.SystemConfig.Dirs.Logs.GetLocalPath()))
        {
            var file = new FileInfo(filePath);
            list.Add(new LogFile
            {
                Name = Path.GetFileName(file.Name),
                Size = file.Length
            });
        }
        return list
            .OrderByDescending(x => x.Name)
            .Take(config.UserConfig.KeepLogsCount);
    }

    public async Task<MemoryStream> GetLogStream(string fileName, bool anonymous)
    {
        var config = _configurationService.Get();
        var logStream = new FileStream(config.SystemConfig.Dirs.Logs.GetLocalFilePath(fileName), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        var newLogStream = new MemoryStream();
        if (!anonymous)
        {
            await logStream.CopyToAsync(newLogStream);
            return newLogStream;
        }

        using (var reader = new StreamReader(logStream))
        {
            var writer = new StreamWriter(newLogStream);
            var serverInfo = await _mediaServerService.GetServerInfo(false);

            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                line = line.Replace(config.UserConfig.MediaServer.Address, "https://xxx.xxx.xxx.xxx:xxxx");
                line = line.Replace(config.UserConfig.MediaServer.FullSocketAddress, "wss://xxx.xxx.xxx.xxx:xxxx");
                line = line.Replace(config.UserConfig.Tmdb.ApiKey, "xxxxxxxxxxxxxx");
                line = line.Replace(config.UserConfig.MediaServer.ApiKey, "xxxxxxxxxxxxxx");
                line = line.Replace(serverInfo.Id, "xxxxxxxxxxxxxx");
                await writer.WriteLineAsync(line);
            }

            await writer.FlushAsync();
        }

        newLogStream.Seek(0, SeekOrigin.Begin);
        return newLogStream;
    }
}