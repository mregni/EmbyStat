using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EmbyStat.Common.Settings;
using EmbyStat.Services.Interfaces;
using EmbyStat.Services.Models.Logs;
using Microsoft.Extensions.Options;

namespace EmbyStat.Services
{
    public class LogService : ILogsService
    {
        private readonly IOptions<LogSettings> _logSettings;

        public LogService(IOptions<LogSettings> logSettings)
        {
            _logSettings = logSettings;
        }

        public List<LogFile> GetLogFileList()
        {
            var list = new List<LogFile>();
            var logDir = _logSettings.Value.Directory;
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
            return list.OrderByDescending(x => x.CreatedDate).Take(20).ToList();
        }

        public Stream GetLogStream(string fileName)
        {
            var logDir = _logSettings.Value.Directory;
            return new FileStream($"{logDir}/{fileName}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }
    }
}
