using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EmbyStat.Common.Extensions;
using EmbyStat.Services.Interfaces;
using EmbyStat.Services.Models.Logs;

namespace EmbyStat.Services;

public class LogService : ILogService
{
    private readonly ISettingsService _settingsService;
    private readonly IMediaServerService _mediaServerService;

    public LogService(ISettingsService settingsService, IMediaServerService mediaServerService)
    {
        _settingsService = settingsService;
        _mediaServerService = mediaServerService;
    }

    public List<LogFile> GetLogFileList()
    {
        var settings = _settingsService.GetUserSettings();
        var list = new List<LogFile>();
        var dirs = _settingsService.GetAppSettings().Dirs;
        foreach (var filePath in Directory.EnumerateFiles(Path.Combine(dirs.Config, dirs.Logs).GetLocalPath()))
        {
            var file = new FileInfo(filePath);
            list.Add(new LogFile
            {
                FileName = Path.GetFileName(file.Name),
                Size = file.Length
            });
        }
        return list.OrderByDescending(x => x.FileName).Take(settings.KeepLogsCount).ToList();
    }

    public async Task<MemoryStream> GetLogStream(string fileName, bool anonymous)
    {
        var dirs = _settingsService.GetAppSettings().Dirs;
        var logStream = new FileStream(Path.Combine(dirs.Config, dirs.Logs, fileName).GetLocalPath(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        var newLogStream = new MemoryStream();
        if (!anonymous)
        {
            await logStream.CopyToAsync(newLogStream);
            return newLogStream;
        }

        using (var reader = new StreamReader(logStream))
        {
            var writer = new StreamWriter(newLogStream);
            var configuration = _settingsService.GetUserSettings();
            var serverInfo = await _mediaServerService.GetServerInfo(false);

            string line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                line = line.Replace(configuration.MediaServer.Address, "https://xxx.xxx.xxx.xxx:xxxx");
                line = line.Replace(configuration.MediaServer.FullSocketAddress, "wss://xxx.xxx.xxx.xxx:xxxx");
                line = line.Replace(configuration.Tmdb.ApiKey, "xxxxxxxxxxxxxx");
                line = line.Replace(configuration.MediaServer.ApiKey, "xxxxxxxxxxxxxx");
                line = line.Replace(serverInfo.Id, "xxxxxxxxxxxxxx");
                await writer.WriteLineAsync(line);
            }

            await writer.FlushAsync();
        }

        newLogStream.Seek(0, SeekOrigin.Begin);
        return newLogStream;
    }
}