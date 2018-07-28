using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EmbyStat.Common;
using EmbyStat.Common.Settings;
using EmbyStat.Services.Interfaces;
using EmbyStat.Services.Models.Logs;
using Microsoft.Extensions.Options;

namespace EmbyStat.Services
{
    public class LogService : ILogsService
    {
        private readonly IOptions<AppSettings> _logSettings;
        private readonly IConfigurationService _configurationService;

        public LogService(IOptions<AppSettings> logSettings, IConfigurationService configurationService)
        {
            _logSettings = logSettings;
            _configurationService = configurationService;
        }

        public List<LogFile> GetLogFileList()
        {
            var configration = _configurationService.GetServerSettings();
            var list = new List<LogFile>();
            var logDir = _logSettings.Value.Logging.Directory;
            foreach (var filePath in Directory.EnumerateFiles(logDir))
            {
                var file = new FileInfo(filePath);
                list.Add(new LogFile
                {
                    FileName = file.Name,
                    CreatedDate = file.CreationTime,
                    Size = file.Length
                });
            }
            return list.OrderByDescending(x => x.CreatedDate).Take(configration.KeepLogsCount).ToList();
        }

        public Stream GetLogStream(string fileName, bool anonymous)
        {
            var logDir = _logSettings.Value.Logging.Directory;
            var logStream = new FileStream($"{logDir}/{fileName}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            if (!anonymous)
            {
                return logStream;
            }

            var newLogStream = new MemoryStream();
            using (var reader = new StreamReader(logStream))
            {
                var writer = new StreamWriter(newLogStream);
                var configuration = _configurationService.GetServerSettings();
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Replace(configuration.EmbyServerAddress, "http://xxx.xxx.xxx.xxx:xxxx");
                    line = line.Replace(configuration.TvdbApiKey, "xxxxxxxxxxxxxx");
                    line = line.Replace(configuration.EmbyUserName, "{EMBY ADMIN USER}");
                    writer.WriteLine(line);
                }

                newLogStream.Seek(0, SeekOrigin.Begin);
                return newLogStream;
            }
        }
    }
}
