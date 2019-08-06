using System.Collections.Generic;
using System.IO;
using System.Linq;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Services.Interfaces;
using EmbyStat.Services.Models.Logs;
using Microsoft.Extensions.Options;

namespace EmbyStat.Services
{
    public class LogService : ILogService
    {
        private readonly ISettingsService _settingsService;

        public LogService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
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
                    FileName = Path.GetFileNameWithoutExtension(file.Name),
                    CreatedDate = file.CreationTime,
                    Size = file.Length
                });
            }
            return list.OrderByDescending(x => x.CreatedDate).Take(settings.KeepLogsCount).ToList();
        }

        public Stream GetLogStream(string fileName, bool anonymous)
        {
            var dirs = _settingsService.GetAppSettings().Dirs;
            var logStream = new FileStream(Path.Combine(dirs.Config, dirs.Logs, fileName).GetLocalPath(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            if (!anonymous)
            {
                return logStream;
            }

            var newLogStream = new MemoryStream();
            using (var reader = new StreamReader(logStream))
            {
                var writer = new StreamWriter(newLogStream);
                var configuration = _settingsService.GetUserSettings();
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Replace(configuration.FullEmbyServerAddress, "http://xxx.xxx.xxx.xxx:xxxx");
                    line = line.Replace(configuration.Tvdb.ApiKey, "xxxxxxxxxxxxxx");
                    line = line.Replace(configuration.Emby.UserName, "{EMBY ADMIN USER}");
                    writer.WriteLine(line);
                }

                writer.Flush();
            }

            newLogStream.Seek(0, SeekOrigin.Begin);
            return newLogStream;
        }
    }
}
