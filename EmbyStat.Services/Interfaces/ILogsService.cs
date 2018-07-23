using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using EmbyStat.Services.Models.Logs;

namespace EmbyStat.Services.Interfaces
{
    public interface ILogsService
    {
        List<LogFile> GetLogFileList();
        Stream GetLogStream(string fileName, bool anonymous);
    }
}
